using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MCProtocol
{

    public class ProtocolRunner
    {
        ushort TipNo = 0;
        ushort TipNoOld = 0;
        ushort TipNoGet()
        {
            TipNoOld = (ushort)(TipNo + 1);
            TipNo = (ushort)((TipNo + 1) % 36);
            return TipNoOld;
        }

        //プロトコル用インデックス
        const int PROT_CMD_W = 0;       //CMD
        const int PROT_ARG1_W = 1;      //リファレンス
        const int PROT_ARG2_W2 = 2;     //リファレンス時のウェイト
        const int PROT_ARG3_W2 = 4;     //引数1
        const int PROT_ARG4_W2 = 6;     //引数2
        const int PROT_ARG5_W2 = 8;     //引数3
        const int PROT_ARG6_W2 = 10;    //引数4
        const int PROT_ARG7_W2 = 12;    //引数5
        const int PROT_ARG8_W2 = 14;    //引数6
        const int PROT_ARG9_W2 = 16;    //引数7
        const int PROT_ARG10_W2 = 18;   //引数8

        public IEnumerable<bool> Start(PLCDevice plc, IUIUpdatable ui)
        {
            var stopwatch = new Stopwatch();
            while (true)
            {
                //設定情報
                var kin1_cnt = new ushort[] { 0, 0, 0, 0, 0, 0, 0 };
                var kin1_z1 = new ushort[] { 15001, 15002, 15003, 15004, 15005, 15006, 15007 };

                //プロトコル開始待ち
                while (!plc.ReadMR(4310))
                    yield return true;
                plc.WriteMR(4311, true);
                ui.AddCommLog("", "◆プロトコル開始");
                stopwatch.Restart();
                plc.WriteMR(4310, false);

                //プロトコル実行
                var cur = DateTime.Now;

                for (var adr = 30000; adr < 90000; adr += 20)
                {
                    stopwatch.Restart();
                    var old = cur;
                    var cmd = plc.ReadDM(adr);
                    var ret = new ushort[20];
                    var no = (ushort)((adr - 30000) / 20);

                    ui.AddCommLog("", $"[{no}]{cmd}実行");

                    switch (cmd)
                    {
                        case 1: //"PVER";"「１」：プロトコルバージョン",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 0:引数3のバージョン数値	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG4_W2);       // 1:引数4のバージョン数値	
                            ret[2] = (ushort)plc.ReadDM2(adr + PROT_ARG5_W2);       // 2:引数5のバージョン数値
                            break;

                        case 2: //"MVIP";"「２」：原点復帰",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:原点復帰動作時のエラーNo
                            break;

                        case 3: //"LOPT";"「３」：ピペットチップ装着",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ピペットチップ装着時のエラーコード
                            ret[1] = TipNoGet();                                    // 1:使用したチップ位置	
                            ret[2] = 0;                                             // 2:ピペット１エラーNo	
                            ret[3] = 0;                                             // 3:ピペット2エラーNo	
                            ret[4] = 0;                                             // 4:ピペット3エラーNo	
                            ret[5] = 0;                                             // 5:ピペット4エラーNo	
                            ret[6] = 0;                                             // 6:ピペット5エラーNo	
                            ret[7] = 0;                                             // 7:ピペット6エラーNo	
                            ret[8] = 3101;                                          // 8:ピペット1装着時大気圧AD値	
                            ret[9] = 3102;                                          // 9:ピペット2着時大気圧AD値	
                            ret[10] = 3103;                                         //10:ピペット3装着時大気圧AD値	
                            ret[11] = 3104;                                         //11:ピペット4装着時大気圧AD値	
                            ret[12] = 3105;                                         //12:ピペット5装着時大気圧AD値	
                            ret[13] = 3106;                                         //13:ピペット6装着時大気圧AD値
                            break;

                        case 4: //"DSPT";"「４」：ピペットチップ廃棄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ピペットチップ廃棄時のエラーコード	
                            ret[1] = TipNoOld;                                      // 1:使用したチップ位置	
                            ret[2] = 0;                                             // 2:ピペット１エラーNo	
                            ret[3] = 0;                                             // 3:ピペット2エラーNo	
                            ret[4] = 0;                                             // 4:ピペット3エラーNo	
                            ret[5] = 0;                                             // 5:ピペット4エラーNo	
                            ret[6] = 0;                                             // 6:ピペット5エラーNo	
                            ret[7] = 0;                                             // 7:ピペット6エラーNo
                            break;

                        case 5: //"SNPT";"「５」：ノズル先端検知",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ノズル先端検知時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のモード	
                            ret[2] = 0;                                             // 2:モード指定ノズル1エラーNo	
                            ret[3] = 0;                                             // 3:モード指定ノズル2エラーNo	
                            ret[4] = 0;                                             // 4:モード指定ノズル3エラーNo	
                            ret[5] = 0;                                             // 5:モード指定ノズル4エラーNo	
                            ret[6] = 0;                                             // 6:モード指定ノズル5エラーNo	
                            ret[7] = 0;                                             // 7:モード指定ノズル6エラーNo	
                            ret[8] = 10001;                                         // 8:モード指定ノズル1先端高さ	
                            ret[9] = 10002;                                         // 9:モード指定ノズル2先端高さ	
                            ret[10] = 10003;                                        //10:モード指定ノズル3先端高さ	
                            ret[11] = 10004;                                        //11:モード指定ノズル4先端高さ	
                            ret[12] = 10005;                                        //12:モード指定ノズル5先端高さ	
                            ret[13] = 10006;                                        //13:モード指定ノズル6先端高さ
                            break;

                        case 6: //"NOWA";"「６」：ノズル洗浄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ノズル洗浄時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のモード	
                            ret[2] = 0;                                             // 2:モード指定ノズル1エラーNo	
                            ret[3] = 0;                                             // 3:モード指定ノズル2エラーNo	
                            ret[4] = 0;                                             // 4:モード指定ノズル3エラーNo	
                            ret[5] = 0;                                             // 5:モード指定ノズル4エラーNo	
                            ret[6] = 0;                                             // 6:モード指定ノズル5エラーNo	
                            ret[7] = 0;                                             // 7:モード指定ノズル6エラーNo
                            break;

                        case 7: //"NODR";"「７」：ノズル乾燥",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ノズル乾燥時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のモード	
                            ret[2] = 0;                                             // 2:モード指定ノズル1エラーNo	
                            ret[3] = 0;                                             // 3:モード指定ノズル2エラーNo	
                            ret[4] = 0;                                             // 4:モード指定ノズル3エラーNo	
                            ret[5] = 0;                                             // 5:モード指定ノズル4エラーNo	
                            ret[6] = 0;                                             // 6:モード指定ノズル5エラーNo	
                            ret[7] = 0;                                             // 7:モード指定ノズル6エラーNo
                            break;

                        case 8: //"LDRG";"「８」：試薬液面検知",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬液面検知時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3の試薬リザーバ番号
                            ret[2] = (ushort)plc.ReadDM2(adr + PROT_ARG6_W2);       // 2:引数6の使用ノズル指定	
                            ret[3] = 0;                                             // 3:使用ノズル1エラーコード	
                            ret[4] = 0;                                             // 4:使用ノズル2エラーコード	
                            ret[5] = 0;                                             // 5:使用ノズル3エラーコード	
                            ret[6] = 0;                                             // 6:使用ノズル4エラーコード	
                            ret[7] = 0;                                             // 7:使用ノズル5エラーコード	
                            ret[8] = 0;                                             // 8:使用ノズル6エラーコード	
                            ret[9] = (ushort)(11001 + ret[1] * 1000);               // 9:使用ノズル1液面高さ	
                            ret[10] = (ushort)(11002 + ret[1] * 1000);              //10:使用ノズル2液面高さ	
                            ret[11] = (ushort)(11003 + ret[1] * 1000);              //11:使用ノズル3液面高さ	
                            ret[12] = (ushort)(11004 + ret[1] * 1000);              //12:使用ノズル4液面高さ	
                            ret[13] = (ushort)(11005 + ret[1] * 1000);              //13:使用ノズル5液面高さ	
                            ret[14] = (ushort)(11006 + ret[1] * 1000);              //14:使用ノズル5液面高さ
                            break;

                        case 9: //"STRG";"「９」：試薬撹拌",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬攪拌時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG8_W2);       // 1:引数8の使用ノズル指定	
                            ret[2] = 0;                                             // 2:使用ノズル1エラーコード	
                            ret[3] = 0;                                             // 3:使用ノズル2エラーコード	
                            ret[4] = 0;                                             // 4:使用ノズル3エラーコード	
                            ret[5] = 0;                                             // 5:使用ノズル4エラーコード	
                            ret[6] = 0;                                             // 6:使用ノズル5エラーコード	
                            ret[7] = 0;                                             // 7:使用ノズル6エラーコード
                            break;

                        case 10: //"SCRG"; "「10」：試薬吸引",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬吸引時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のリザーバ番号	
                            ret[2] = 0;                                             // 2:ピペット1エラーNo	
                            ret[3] = 0;                                             // 3:ピペット2エラーNo	
                            ret[4] = 0;                                             // 4:ピペット3エラーNo	
                            ret[5] = 0;                                             // 5:ピペット4エラーNo	
                            ret[6] = 0;                                             // 6:ピペット5エラーNo	
                            ret[7] = 0;                                             // 7:ピペット6エラーNo	

                            var sno = ret[1] - 1;
                            var z1 = kin1_z1[sno];
                            kin1_z1[sno] += 50;
                            ret[8] = kin1_cnt[sno]++;                               // 8:試薬リザーバ吸引回数	
                            ret[9] = (ushort)(z1 + 1);                              // 9:ピペット１吸引高さ	
                            ret[10] = (ushort)(z1 + 2);                             //10:ピペット２吸引高さ	
                            ret[11] = (ushort)(z1 + 3);                             //11:ピペット３吸引高さ	
                            ret[12] = (ushort)(z1 + 4);                             //12:ピペット４吸引高さ	
                            ret[13] = (ushort)(z1 + 5);                             //13:ピペット５吸引高さ	
                            ret[14] = (ushort)(z1 + 6);                             //14:ピペット６吸引高さ
                            break;

                        case 11: //"DSRG"; "「11」：試薬吐出",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬吐出時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のトレイ番号	
                            ret[2] = 0;                                             // 2:トレイ番号にセットされているトレイQR	
                            ret[3] = (ushort)plc.ReadDM2(adr + PROT_ARG4_W2);       // 3:引数4の処理行	
                            ret[4] = 0;                                             // 4:ピペット1エラーNo	
                            ret[5] = 0;                                             // 5:ピペット2エラーNo	
                            ret[6] = 0;                                             // 6:ピペット3エラーNo	
                            ret[7] = 0;                                             // 7:ピペット4エラーNo	
                            ret[8] = 0;                                             // 8:ピペット5エラーNo	
                            ret[9] = 0;                                             // 9:ピペット6エラーNo	
                            ret[10] = 9991;                                         //10:ピペット１吐出高さ	
                            ret[11] = 9992;                                         //11:ピペット２吐出高さ	
                            ret[12] = 9993;                                         //12:ピペット３吐出高さ	
                            ret[13] = 9994;                                         //13:ピペット４吐出高さ	
                            ret[14] = 9995;                                         //14:ピペット５吐出高さ	
                            ret[15] = 9996;                                         //15:ピペット６吐出高さ
                            break;

                        case 12: //"SDRG"; "「12」：試薬吐出後攪拌",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬吐出後攪拌時のエラーコード	
                            ret[1] = 0;                                             // 1:ピペット1エラーNo	
                            ret[2] = 0;                                             // 2:ピペット2エラーNo	
                            ret[3] = 0;                                             // 3:ピペット3エラーNo	
                            ret[4] = 0;                                             // 4:ピペット4エラーNo	
                            ret[5] = 0;                                             // 5:ピペット5エラーNo	
                            ret[6] = 0;                                             // 6:ピペット6エラーNo
                            break;

                        case 13: //"DCRG"; "「13」：試薬廃液",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:試薬廃液エラーコード	
                            ret[1] = 0;                                             // 1:ピペット1エラーNo	
                            ret[2] = 0;                                             // 2:ピペット2エラーNo	
                            ret[3] = 0;                                             // 3:ピペット3エラーNo	
                            ret[4] = 0;                                             // 4:ピペット4エラーNo	
                            ret[5] = 0;                                             // 5:ピペット5エラーNo	
                            ret[6] = 0;                                             // 6:ピペット6エラーNo
                            break;

                        case 14: //"WACP"; "「14」：液溜め洗浄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:液溜め洗浄時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のトレイ番号	
                            ret[2] = 0;                                             // 2:トレイ番号にセットされているトレイQR	
                            ret[3] = (ushort)plc.ReadDM2(adr + PROT_ARG4_W2);       // 3:引数4の処理行	
                            ret[4] = (ushort)plc.ReadDM2(adr + PROT_ARG8_W2);       // 4:引数8の送液ポンプ駆動	
                            ret[5] = 0;                                             // 5:2重管1エラーNo	
                            ret[6] = 0;                                             // 6:2重管2エラーNo	
                            ret[7] = 0;                                             // 7:2重管3エラーNo	
                            ret[8] = 0;                                             // 8:2重管4エラーNo	
                            ret[9] = 0;                                             // 9:2重管5エラーNo	
                            ret[10] = 0;                                            //10:2重管6エラーNo	
                            ret[11] = 1;                                            //11:流量センサ値1	
                            ret[12] = 2;                                            //12:流量センサ値2	
                            ret[13] = 3;                                            //13:流量センサ値3	
                            ret[14] = 4;                                            //14:流量センサ値4	
                            ret[15] = 5;                                            //15:流量センサ値5	
                            ret[16] = 6;                                            //16:流量センサ値6
                            break;

                        case 15: //"CRL1"; "「15」：液溜め残液制御１",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:液溜め残液制御１（OK1A用）時のエラーコード	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のトレイ番号	
                            ret[2] = 0;                                             // 2:トレイ番号にセットされているトレイQR	
                            ret[3] = (ushort)plc.ReadDM2(adr + PROT_ARG4_W2);       // 3:引数4の処理行	
                            ret[4] = (ushort)plc.ReadDM2(adr + PROT_ARG7_W2);       // 4:引数7の回転方向	
                            ret[5] = 0;                                             // 5:2重管1エラーNo	
                            ret[6] = 0;                                             // 6:2重管2エラーNo	
                            ret[7] = 0;                                             // 7:2重管3エラーNo	
                            ret[8] = 0;                                             // 8:2重管4エラーNo	
                            ret[9] = 0;                                             // 9:2重管5エラーNo	
                            ret[10] = 0;                                            //10:2重管6エラーNo
                            break;

                        case 16: //"CRL2"; "「16」：液溜め残液制御２",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:液溜め残液制御２（OK2A用）のエラーコード	
                            ret[1] = 0;                                             // 1:処理しているトレイBCD	
                            ret[2] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 2:引数3の処理行	
                            ret[3] = 0;                                             // 3:2重管1エラーNo	
                            ret[4] = 0;                                             // 4:2重管2エラーNo	
                            ret[5] = 0;                                             // 5:2重管3エラーNo	
                            ret[6] = 0;                                             // 6:2重管4エラーNo	
                            ret[7] = 0;                                             // 7:2重管5エラーNo	
                            ret[8] = 0;                                             // 8:2重管6エラーNo
                            break;

                        case 17: //"STMP"; "「17」：タイムスタンプ",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;     // 0:引数3の20文字
                            break;

                        case 18: //"DSPM"; "「18」：ポンプ廃液",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:ポンプ廃液時のエラーNo	
                            ret[1] = (ushort)plc.ReadDM2(adr + PROT_ARG3_W2);       // 1:引数3のタンク番号	
                            ret[2] = 0;                                             // 2:2重管1エラーNo	
                            ret[3] = 0;                                             // 3:2重管2エラーNo	
                            ret[4] = 0;                                             // 4:2重管3エラーNo	
                            ret[5] = 0;                                             // 5:2重管4エラーNo	
                            ret[6] = 0;                                             // 6:2重管5エラーNo	
                            ret[7] = 0;                                             // 7:2重管6エラーNo
                            break;

                        case 19: //"WAIT"; "「19」：タイマ",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 20: //"SPAR"; "「20」：エア吹付",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            ret[0] = 0;                                             // 0:エア吹付（乾燥）時エラーコード	
                            ret[1] = 0;                                             // 1:2重管1エラーNo	
                            ret[2] = 0;                                             // 2:2重管2エラーNo	
                            ret[3] = 0;                                             // 3:2重管3エラーNo	
                            ret[4] = 0;                                             // 4:2重管4エラーNo	
                            ret[5] = 0;                                             // 5:2重管5エラーNo	
                            ret[6] = 0;                                             // 6:2重管6エラーNo
                            break;

                        case 21: //"PCNT"; "「21」：プロトコルファイル継続",
                            ui.AddCommLog("", "◆PCNT");
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);

                            //PCNT完了待ち
                            while (true)
                            {
                                yield return true;

                                var stage2_406_mr = plc.ReadMR(12606);  //0になるのを待つ
                                if (stage2_406_mr) continue;

                                var tray3_1 = plc.ReadMR(2011);
                                if (!tray3_1) continue;

                                var tray4_1 = plc.ReadMR(2012);
                                if (!tray4_1) continue;

                                //var tray3_1 = plc.ReadMR(2002);
                                //if (!tray3_1) continue;

                                //var tray4_1 = plc.ReadMR(2003);
                                //if (!tray4_1) continue;

                                //var tray3_2 = plc.ReadMR(2006);
                                //if (!tray3_2) continue;

                                //var tray4_2 = plc.ReadMR(2007);
                                //if (!tray4_2) continue;


                                break;
                            }
                            break;

                        case 22: //"PEND"; "「22」：プロトコルファイル終了",
                            ui.AddCommLog("", "◆PEND：ステージ２完了待ち");
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);

                            //プロトコル終了通知
                            ui.AddCommLog("", "◆PEND完了");
                            adr = 9999999;
                            break;

                        case 23: //"S1FN"; "「23」：ステージ1プロトコル終了",
                            ui.AddCommLog("", "◆S1FN");
                            plc.WriteMR(4309, true);
                            cur = cur.AddSeconds(0);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        default:
                            //不明
                            break;
                    }

                    //ウェイト
                    while (stopwatch.ElapsedMilliseconds < 700)
                        yield return true;

                    //バッファ空き待ち                    
                    foreach (var isFull in IsFull(plc))
                        yield return true;

                    //次へ
                    yield return true;
                }

                ui.AddCommLog("", "◆プロトコル完了");
                plc.WriteMR(4311, false);
            }
        }

        static IEnumerable<bool> IsFull(PLCDevice plc)
        {
            //現在の書き込みポインタを読み込み
            while (true)
            {
                var rp = plc.ReadDM(29400);
                var wp = (plc.ReadDM(29401) + 1) % 10;
                if (rp == wp)
                {
                    //満杯
                    yield return true;
                }
                else
                {
                    //空きあり
                    yield break;
                }
            }
        }

        static void InnerLogWrite(PLCDevice plc, ushort no, ushort cmd, DateTime st, DateTime ed, ushort[] data)
        {
            //現在の書き込みポインタを読み込み
            var wp = plc.ReadDM(29401);

            //開始時刻
            var st_hh = st.Hour * 10000;
            var st_mm = st.Minute * 100;
            var st_ss = st.Second * 1;
            var st_dt = (uint)(st_hh + st_mm + st_ss);

            //完了時刻
            var ed_hh = ed.Hour * 10000;
            var ed_mm = ed.Minute * 100;
            var ed_ss = ed.Second * 1;
            var ed_dt = (uint)(ed_hh + ed_mm + ed_ss);

            var address = 29000 + 40 * wp;
            plc.WriteDM(address + 0, no);            //行番号
            plc.WriteDM(address + 1, cmd);           //PID
            plc.WriteDM2(address + 2, st_dt);        //開始時間
            plc.WriteDM2(address + 4, ed_dt);        //終了時間
            plc.WriteDM2(address + 6, data[0]);      //コマンド1
            plc.WriteDM2(address + 8, data[1]);      //コマンド2
            plc.WriteDM2(address + 10, data[2]);     //コマンド3
            plc.WriteDM2(address + 12, data[3]);     //コマンド4
            plc.WriteDM2(address + 14, data[4]);     //コマンド5
            plc.WriteDM2(address + 16, data[5]);     //コマンド6
            plc.WriteDM2(address + 18, data[6]);     //コマンド7
            plc.WriteDM2(address + 20, data[7]);     //コマンド8
            plc.WriteDM2(address + 22, data[8]);     //コマンド9
            plc.WriteDM2(address + 24, data[9]);     //コマンド10
            plc.WriteDM2(address + 26, data[10]);    //コマンド11
            plc.WriteDM2(address + 28, data[11]);    //コマンド12
            plc.WriteDM2(address + 30, data[12]);    //コマンド13
            plc.WriteDM2(address + 32, data[13]);    //コマンド14
            plc.WriteDM2(address + 34, data[14]);    //コマンド15
            plc.WriteDM2(address + 36, data[15]);    //コマンド16
            plc.WriteDM2(address + 38, data[16]);    //コマンド17

            //次の書き込みポインタへ移動
            var np = (ushort)((wp + 1) % 10);
            plc.WriteDM(29401, np);
        }
    }

    /// <summary>
    /// PLCデバイス
    /// </summary>
    public class PLCDevice
    {
        public readonly ConcurrentDictionary<int, bool> R = new();
        public readonly ConcurrentDictionary<int, bool> MR = new();
        public readonly ConcurrentDictionary<int, ushort> DM = new();
        readonly ConcurrentDictionary<int, bool> _R = new();
        readonly ConcurrentDictionary<int, bool> _MR = new();
        readonly ConcurrentDictionary<int, ushort> _DM = new();

        readonly Task DoTask;
        readonly Task DoPluginsTask;
        public bool IsFinished => DoTask.IsCompleted;

        void Test()
        {
#if false
            //ビット操作の確認
            R.SetBit(1000, 1, new byte[] { 0x10 });
            var ret0 = R.GetWord(1000, 1);
            R.SetBit(1001, 2, new byte[] { 0x01 });
            R.SetBit(1003, 2, new byte[] { 0x11 });
            var ret1 = R.GetBit(1000, 1);
            var ret2 = R.GetBit(1001, 1);
            var ret3 = R.GetBit(1000, 5);

            R.SetWord(1500, 1, new byte[] { 0x01, 0x20 });
            var retA = R.GetWord(1500, 1);
            var retB = R.GetBit(1500, 1);
            var retC = R.GetBit(1500 + 12, 2);
            var retD = R.GetBit(1500 + 13, 1);

            //ワード操作の確認
            R.SetWord(2000, 2, new[] { (byte)0xAB, (byte)0x12, (byte)0x34, (byte)0xCD });
            var ret4 = R.GetWord(2000, 2);
            var keys = R.Keys.ToList();
            keys.Sort();
            foreach (var key in keys)
            {
                var flg = R[key] ? "●" : "－";
                Debug.WriteLine($"{key}:{flg}");
            }
            var ret5 = R.GetBit(2000, 32);

            //DM操作の確認
            DM.SetWord(2000, 2, new[] { (byte)0xAB, (byte)0x12, (byte)0x34, (byte)0xCD });
            var ret6 = DM.GetWord(2000, 2);


            //DM.SetWord(2000, 20, new byte[] { 01, 00, 00, 00, 00, 00, 00, 00, 01, 00, 00, 00, 01, 00, 00, 00, 01, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
            DM.SetWord(2000, 20, new byte[] { 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xE8, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            var _cmd = ReadDM(2000);
            var _ref = ReadDM(2001);
            var _wai = ReadDM(2002);
            var _arg1 = ReadDM(2004);
            var _arg2= ReadDM(2006);
            var _arg3 = ReadDM(2008);
            var _arg4 = ReadDM(2010);

            //Thread.Sleep(1000);
            //var ret7 = MR[100];
#endif
        }

        readonly IUIUpdatable Updatable;
        public PLCDevice(int port, IUIUpdatable updatable)
        {
            Updatable = updatable;
            Processor.Execute(this);
            Test();
            DoTask = Start(port);
            DoPluginsTask = PluginsStart();
        }

        Task Start(int port)
        {
            return Task.Run(() =>
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                while (true)
                {
                    Updatable.UpdateConnect(ListenPacket.Count);
                    new ListenPacket(listener.AcceptSocket(), RecieveAndResponse, Updatable).Start();
                }
            });
        }

        Task PluginsStart()
        {
            var plugins = new List<IEnumerator<bool>>()
            {
                new ProtocolRunner().Start(this, Updatable).GetEnumerator(),   //プロトコル実行
            };

            return Task.Run(() =>
            {
                while (true)
                {
                    foreach (var plugin in plugins)
                        plugin.MoveNext();
                    Thread.Sleep(10);
                }
            });
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] RecieveAndResponse(Socket socket, byte[] bytes)
        {
            var res = new List<byte>
            {
                (byte)(0x80 | bytes[0]),                            //フレーム
                bytes[1],
                bytes[2],                                           //ネットワーク番号
                bytes[3],                                           //PC番号
                bytes[4],                                           //IO要求先ユニット
                bytes[5],
                bytes[6]                                            //要求先ユニット局番号
            };

            var dat = Parse(bytes);                                 //データ解析
            var len = dat.Length + 2;
            res.Add((byte)(len & 0xff));                            //L:応答データ長    7
            res.Add((byte)(len >> 8));                              //H                 8
            res.Add(0x00);                                          //終了コード        9
            res.Add(0x00);
            res.AddRange(dat);                                      //データ追記        10

            return res.ToArray();
        }

        static int AddressDecode(int address)
        {
            return ((address >> 4) * 100) + (address & 0xf);
        }

        static int AddressEncode(int address)
        {
            return ((address / 100) << 4) + (address % 100);
        }


        byte[] Parse(byte[] bytes)
        {
            //var length = bytes[7] + (bytes[8] << 8);              //受信データ長
            //var timer = bytes[9] + (bytes[10] << 8);              //CPU監視タイマ
            var cmd = bytes[11] + (bytes[12] << 8);                 //コマンド
            var sub = bytes[13] + (bytes[14] << 8);                 //サブコマンド
            //データ部
            var adr = bytes[15] | bytes[16] << 8 | bytes[17] << 16; //アドレス
            var dev = bytes[18];                                    //デバイスコード
            var len = bytes[19] | bytes[20] << 8;                   //デバイス数
            var siz = sub == 1 ? 1 : 2;
            var dat = bytes[21..(21 + len * siz)];                  //受信データ

            var _adr = adr;
            adr = AddressDecode(adr);
            switch (cmd)
            {
                case 0x0401:
                    //一括読み込み
                    if (dev == 0x9c && sub == 0)
                    {
                        var resp = R.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"R{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x9c && sub == 1)
                    {
                        var resp = R.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"R{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 0)
                    {
                        var resp = MR.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"MR{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 1)
                    {
                        var resp = MR.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"MR{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0xA8 && sub == 0)
                    {
                        adr = _adr;
                        var resp = DM.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"DM{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    break;

                case 0x1401:
                    //一括書き込み
                    try
                    {
                        var body = string.Join(" ", dat.Select(_ => _.ToString("X2")));
                        if (dev == 0x9c && sub == 0)
                        {
                            Updatable.AddCommLog($"R{adr}", $"Write:len={len}, dat={body}");
                            return R.SetWord(adr, len, dat);
                        }
                        if (dev == 0x9c && sub == 1)
                        {
                            Updatable.AddCommLog($"R{adr}", $"Write: R  len={len}, dat={body}");
                            return R.SetBit(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 0)
                        {
                            Updatable.AddCommLog($"MR{adr}", $"Write:len={len}, dat={body}");
                            return MR.SetWord(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 1)
                        {
                            Updatable.AddCommLog($"MR{adr}", $"Write:len={len}, dat={body}");
                            return MR.SetBit(adr, len, dat);
                        }
                        if (dev == 0xA8 && sub == 0)
                        {
                            adr = _adr;
                            Updatable.AddCommLog($"DM{adr}", $"Write:len={len}, dat={body}");
                            return DM.SetWord(adr, len, dat);
                        }
                    }
                    finally
                    {
                        Processor.Execute(this);
                    }
                    break;

                default:
                    break;
            }
            return Array.Empty<byte>();
        }

        public bool ReadR(int address) => MR.TryGetValue(address, out bool value) && value;
        public bool ReadMR(int address) => MR.TryGetValue(address, out bool value) && value;
        public ushort ReadDM(int address) => DM.TryGetValue(address, out ushort value) ? value : (ushort)0;
        public uint ReadDM2(int address)
        {
            var l = DM.TryGetValue(address + 0, out ushort valueL) ? valueL : (ushort)0;
            var h = DM.TryGetValue(address + 1, out ushort valueH) ? valueH : (ushort)0;
            var ret = (uint)((h << 16) + l);
            return ret;
        }

        public void WriteR(int address, bool flg)
        {
            R[address] = flg;
            Updatable.AddCommLog($" R{address}", $"Write {flg}");
        }
        public void WriteMR(int address, bool flg)
        {
            MR[address] = flg;
            Updatable.AddCommLog($"MR{address}", $"Write {flg}");
        }
        public void WriteDM(int address, ushort value)
        {
            DM[address] = value;
            Updatable.AddCommLog($"DM{address}", $"Write {value}");
        }
        public void WriteDM2(int address, uint value)
        {
            var l = (ushort)(value & 0xffff);
            var h = (ushort)((value >> 16) & 0xffff);
            WriteDM(address + 0, l);
            WriteDM(address + 1, h);
        }

    }
}