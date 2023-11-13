using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace MCProtocol
{
    public static class Processor
    {
        readonly static ConcurrentBag<string> IgnoreFilenames = new();
        public static void Execute(PLCDevice plc, string path)
        {
            var files = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                Dispatch(plc, file);
            }
        }

        static Task Dispatch(PLCDevice plc, string filename)
        {
            return Task.Run(() =>
            {
                if (IgnoreFilenames.Contains(filename))
                    return;

                var lines = File.ReadAllLines(filename)
                    .Select(_ => Regex.Replace(_, "[ 　\t]", ""))
                    .Select(_ => Regex.Replace(_, ";.*$", "").ToUpper());

                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                    var rows = line.Split(',');
                    if (rows is null) return;
                    if (string.IsNullOrEmpty(rows[0])) return;
                    switch (rows[0])
                    {
                        case "INIT":
                            IgnoreFilenames.Add(filename);
                            break;

                        case "TRIG":
                            switch (rows[1])
                            {
                                case "R":
                                    if (plc.R.TryGetValue(int.Parse(rows[2]), out bool rv))
                                    {
                                        var flg = rows[3] == "1";
                                        if (flg != rv) return;
                                        continue;
                                    }
                                    break;

                                case "MR":
                                    if (plc.MR.TryGetValue(int.Parse(rows[2]), out bool mrv))
                                    {
                                        var flg = rows[3] == "1";
                                        if (flg != mrv) return;
                                        continue;
                                    }
                                    break;

                                case "DM":
                                    if (plc.DM.TryGetValue(int.Parse(rows[2]), out ushort dmv))
                                    {
                                        var flg = int.Parse(rows[3]);
                                        if (flg != dmv) return;
                                        continue;
                                    }
                                    break;

                                default:
                                    throw new Exception("未定義の引数です");
                            }
                            break;

                        case "WAIT":
                            var waitValue = int.Parse(rows[1]);
                            if (waitValue > 0) Thread.Sleep(waitValue);
                            break;

                        case "CLR":
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
                    }
                }
            });
        }
    }
}
