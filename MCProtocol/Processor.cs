using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MCProtocol
{
    public static class Processor
    {
        public static bool IsRandomMode = false;
        readonly static ConcurrentBag<string> IgnoreFilenames = new();
        readonly static ConcurrentQueue<IEnumerator<bool>> Queues = new();
        readonly static List<string> Filenames = new();
        static bool IsPower = false;
        static IUIUpdatable? Updatable;

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="plc"></param>
        public static void Execute(PLCDevice plc)
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories).Where(_ => !IgnoreFilenames.Contains(_)).Where(_ => !Filenames.Contains(_));
            foreach (var file in files)
            {
                Filenames.Add(file);
                Queues.Enqueue(Dispatch(plc, file).GetEnumerator());
            }

            if (Queues.TryDequeue(out IEnumerator<bool>? queue))
            {
                if (queue.MoveNext())
                    Queues.Enqueue(queue);
            }
        }

        /// <summary>
        /// タスク起動
        /// </summary>
        /// <param name="plc"></param>
        public static void Start(PLCDevice plc, IUIUpdatable updatable)
        {
            Updatable = updatable;
            new Thread(() =>
            {
                IsPower = true;
                while (IsPower)
                {
                    Execute(plc);
                    Thread.Sleep(1);
                }
            }).Start();
        }

        public static void Stop()
        {
            IsPower = false;
        }

        /// <summary>
        /// ファイル検索
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static string? SearchFile(string filename)
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.csv", SearchOption.AllDirectories);
            return files.FirstOrDefault(_=> Path.GetFileName(_).Contains(filename, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// ファイル実行
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static IEnumerable<bool> Dispatch(PLCDevice plc, string filename)
        {
            //if (IgnoreFilenames.Contains(filename))
            //    yield break;
            //if (Filenames.Contains(filename))
            //    yield break;

            //Filenames.Add(filename);
            var lines = File.ReadAllLines(filename, Encoding.UTF8)
                .Select(_ => Regex.Replace(_, "[ 　\t]", ""))
                .Select(_ => Regex.Replace(_, ";.*$", "").ToUpper());

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                var rows = line.Split(',');
                if (rows is null) break;
                if (string.IsNullOrEmpty(rows[0])) break;
                switch (rows[0])
                {
                    case "INIT":
                        //１回のみ実行
                        IgnoreFilenames.Add(filename);
                        break;

                    case "TRIG":
                        //条件チェック（一致したらスクリプト実行）
                        while (true)
                        {
                            var isLoop = true;
                            switch (rows[1])
                            {
                                case "R":
                                    if (plc.R.TryGetValue(int.Parse(rows[2]), out bool rv))
                                    {
                                        var flg = rows[3] == "1";
                                        isLoop = flg != rv;
                                    }
                                    break;

                                case "MR":
                                    if (plc.MR.TryGetValue(int.Parse(rows[2]), out bool mrv))
                                    {
                                        var flg = rows[3] == "1";
                                        isLoop = flg != mrv;
                                    }
                                    break;

                                case "DM":
                                    if (plc.DM.TryGetValue(int.Parse(rows[2]), out ushort dmv))
                                    {
                                        var flg = int.Parse(rows[3]);
                                        isLoop = flg != dmv;
                                    }
                                    break;

                                default:
                                    throw new Exception("未定義の引数です");
                            }

                            if (!isLoop)
                            {
                                //条件一致
                                Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {rows[1]}{rows[2]}");
                                break;
                            }
                            else
                            {
                                //次へ
                                yield return true;
                            }
                        }
                        break;

                    case "CMP":
                        //一致していたら処理継続、不一致なら処理終了
                        var is_compare = false;
                        switch (rows[1])
                        {
                            case "R":
                                is_compare = (plc.R.TryGetValue(int.Parse(rows[2]), out bool rv) && rv) == (rows[3] == "1");
                                break;

                            case "MR":
                                is_compare = (plc.MR.TryGetValue(int.Parse(rows[2]), out bool mrv) && mrv) == (rows[3] == "1");
                                break;

                            case "DM":
                                is_compare = (plc.DM.TryGetValue(int.Parse(rows[2]), out ushort drv) ? drv : 0) == int.Parse(rows[3]);
                                break;

                            default:
                                throw new Exception("未定義の引数です");
                        }

                        if (is_compare)
                        {
                            //一致ならば次へ
                            yield return true;
                        }
                        else
                        {
                            //不一致ならば終了
                            Filenames.Remove(filename);
                            yield break;
                        }
                        break;

                    case "WAIT":
                        //ウェイト
                        var waitValue = int.Parse(rows[1]);
                        var sw = new Stopwatch();
                        sw.Restart();
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {rows[1]}");
                        while (sw.ElapsedMilliseconds < waitValue)
                            yield return true;
                        break;

                    case "CLR":
                        //フラグクリア
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {line}");
                        switch (rows[1])
                        {
                            case "R":
                                plc.R[int.Parse(rows[2])] = false;
                                break;

                            case "MR":
                                plc.MR[int.Parse(rows[2])] = false;
                                break;

                            case "DM":
                                throw new Exception("使用不可");

                            default:
                                throw new Exception("未定義の引数です");
                        }
                        break;

                    case "SET":
                        //フラグセット
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {line}");
                        switch (rows[1])
                        {
                            case "R":
                                plc.R[int.Parse(rows[2])] = true;
                                break;

                            case "MR":
                                plc.MR[int.Parse(rows[2])] = true;
                                break;

                            case "DM":
                                plc.DM[int.Parse(rows[2])] = ushort.Parse(rows[3]);
                                break;

                            default:
                                throw new Exception("未定義の引数です");
                        }
                        break;

                    case "CALL":
                        //ファイル実行
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {line}");
                        if (SearchFile(rows[1]) is string _filename)
                        {
                            foreach (var x in Dispatch(plc, _filename))
                                yield return true;
                        }
                        break;

                    case "COPY":
                        //DMコピー
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {line}");
                        var src = int.Parse(rows[1]);
                        var dst = int.Parse(rows[2]);
                        var len = int.Parse(rows[3]);
                        for (var i = 0; i < len; i++)
                        {
                            plc.DM[dst + i] = (plc.DM.TryGetValue(src + i, out ushort val)) ? val : (ushort)0;
                        }
                        break;

                    case "MESG":
                        //コメント
                        Updatable?.AddCommLog("", $"★{Path.GetFileNameWithoutExtension(filename)}: {rows[1]}");
                        break;

                    case "OK1A":
                        //OK1Aのみ
                        if (!MemISDic.IsOK2AMode)
                            yield return true;
                        yield break;

                    case "OK2A":
                        //OK2Aのみ
                        if (MemISDic.IsOK2AMode)
                            yield return true;
                        yield break;
                }
            }

            Filenames.Remove(filename);
        }
    }
}
