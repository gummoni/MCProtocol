namespace MCProtocol
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mult_Reagent1.entity;
using Mult_Reagent1.Log;

namespace Mult_Reagent1
{
	public class plcLog
	{
		public static List<PlcInf> batchtrayLogs = new List<PlcInf>();

		public static int rcurno = 0;

		public static int wcurno = 0;

		public static int Log_pos = 29000;

		public static int P_END = 0;

		public static int S1_END = 0;

		private static int debugno = 0;

		private static int debugno1 = 0;

		public void batchtrayLogs_clear()
		{
			batchtrayLogs.Clear();
		}

		public void P_END_Clear()
		{
			P_END = 0;
		}

		private static int GetSeqData(ref int r, ref int w)
		{
			byte[] array = new byte[60];
			plcio plcio2 = new plcio();
			try
			{
				array = plcio2.ReadDm(1, 608, 2);
				if (((uint)array[9] & (true ? 1u : 0u)) != 0 && ((uint)array[10] & (true ? 1u : 0u)) != 0)
				{
					Mult_Reagent1.Log.Log.Error("ログシーケンス取得に失敗", "GetSeqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 77);
				}
				array = plcio2.ReadDm(1, 29400, 1);
				if (array[9] != 0 || array[10] != 0)
				{
					return -1;
				}
				byte[] array2 = new byte[1] { array[11] };
				byte b = array[11];
				r = b;
				array = plcio2.ReadDm(1, 29401, 1);
				if (array[9] != 0 || array[10] != 0)
				{
					Mult_Reagent1.Log.Log.Error("ログシーケンス取得に失敗", "GetSeqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 103);
					return -1;
				}
				byte[] array3 = new byte[1] { array[11] };
				byte b2 = array[11];
				w = b2;
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("GetSeqData エラー" + ex.ToString(), "GetSeqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 111);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
			return 0;
		}

		private static int GetLogqData(int r, ref PlcInf trayLogs)
		{
			byte[] array = new byte[60];
			int num = 0;
			int num2 = 40;
			int num3 = 0;
			plcio plcio2 = new plcio();
			DateTime now = DateTime.Now;
			string text = now.ToString("yyyy/MM/dd");
			try
			{
				num = Log_pos + num2 * r;
				array = plcio2.ReadDm(1, num, 2);
				if (array[9] != 0 || array[10] != 0)
				{
					return -1;
				}
				byte[] value = new byte[2]
				{
					array[13],
					array[14]
				};
				int num4 = BitConverter.ToInt16(value, 0);
				if (Setting.Value.plsdata.debug_1 == 1)
				{
					DateTime now2 = DateTime.Now;
					string text2 = now.ToString("yyyy/MM/dd");
					debugno++;
					if (debugno == 23)
					{
						debugno = 1;
					}
					num4 = debugno;
				}
				trayLogs.P_Id = num4.ToString();
				if (num4 == 22)
				{
					P_END = 1;
				}
				trayLogs.S_date = text;
				num += 2;
				array = plcio2.ReadDm(1, num, 2);
				if (array[9] != 0 || array[10] != 0)
				{
					return -1;
				}
				byte[] value2 = new byte[4]
				{
					array[11],
					array[12],
					array[13],
					array[14]
				};
				int num5 = BitConverter.ToInt32(value2, 0);
				int num6 = num5 / 10000;
				int num7 = num5 % 10000 / 100;
				int num8 = num5 % 100;
				string s_Time = num6 + ":" + num7 + ":" + num8;
				trayLogs.S_Time = s_Time;
				trayLogs.E_date = text;
				num += 2;
				array = plcio2.ReadDm(1, num, 2);
				if (array[9] != 0 || array[10] != 0)
				{
					return -1;
				}
				byte[] value3 = new byte[4]
				{
					array[11],
					array[12],
					array[13],
					array[14]
				};
				int num9 = BitConverter.ToInt32(value3, 0);
				if (Setting.Value.plsdata.debug_1 == 1 && Setting.Value.plsdata.debug_1 == 1)
				{
					string s = DateTime.Now.ToString("HHmmss");
					int num10 = int.Parse(s);
					num9 = num10;
				}
				int num11 = num9 / 10000;
				int num12 = num9 % 10000 / 100;
				int num13 = num9 % 100;
				string e_Time = num11 + ":" + num12 + ":" + num13;
				if (Setting.Value.plsdata.debug_1 == 1)
				{
				}
				trayLogs.E_Time = e_Time;
				for (int i = 1; i < 17; i++)
				{
					num += 2;
					array = plcio2.ReadDm(1, num, 2);
					if (array[9] == 0 && array[10] == 0)
					{
						byte[] value4 = new byte[4]
						{
							array[11],
							array[12],
							array[13],
							array[14]
						};
						int num14 = BitConverter.ToInt32(value4, 0);
						switch (i)
						{
						case 1:
							trayLogs.argument1 = num14.ToString();
							break;
						case 2:
							trayLogs.argument2 = num14.ToString();
							if (trayLogs.P_Id == "11" || trayLogs.P_Id == "15")
							{
								byte[] value5 = new byte[2]
								{
									array[13],
									array[14]
								};
								num3 = BitConverter.ToInt16(value5, 0);
							}
							break;
						case 3:
							trayLogs.argument3 = num14.ToString();
							if (trayLogs.P_Id == "11" || trayLogs.P_Id == "15")
							{
								if (num3 == 1)
								{
									trayLogs.argument3 = Form1.KosouMode.solid_tray1;
								}
								if (num3 == 2)
								{
									trayLogs.argument3 = Form1.KosouMode.solid_tray2;
								}
								if (num3 == 3)
								{
									trayLogs.argument3 = Form1.KosouMode.solid_tray3;
								}
								if (num3 == 4)
								{
									trayLogs.argument3 = Form1.KosouMode.solid_tray4;
								}
							}
							break;
						case 4:
							trayLogs.argument4 = num14.ToString();
							break;
						case 5:
							trayLogs.argument5 = num14.ToString();
							break;
						case 6:
							trayLogs.argument6 = num14.ToString();
							break;
						case 7:
							trayLogs.argument7 = num14.ToString();
							break;
						case 8:
							trayLogs.argument8 = num14.ToString();
							break;
						case 9:
							trayLogs.argument9 = num14.ToString();
							break;
						case 10:
							trayLogs.argument10 = num14.ToString();
							break;
						case 11:
							trayLogs.argument11 = num14.ToString();
							break;
						case 12:
							trayLogs.argument12 = num14.ToString();
							break;
						case 13:
							trayLogs.argument13 = num14.ToString();
							break;
						case 14:
							trayLogs.argument14 = num14.ToString();
							break;
						case 15:
							trayLogs.argument15 = num14.ToString();
							break;
						case 16:
							trayLogs.argument16 = num14.ToString();
							break;
						case 17:
							trayLogs.argument17 = num14.ToString();
							break;
						}
						continue;
					}
					return -1;
				}
				batchtrayLogs.Add(trayLogs);
				rcurno++;
				if (rcurno == 10)
				{
					rcurno = 0;
				}
				byte[] data = new byte[2]
				{
					(byte)rcurno,
					0
				};
				array = plcio2.WriteDm(1, 29400, data);
				if (array[9] != 0 || array[10] != 0)
				{
					Mult_Reagent1.Log.Log.Error("GetLogqData シーケンスを更新出来なかった。", "GetLogqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 396);
					return -1;
				}
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("GetLogqData エラー" + ex.ToString(), "GetLogqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 417);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
				return -1;
			}
			return 0;
		}

		private static int PlcAnaLogData(PlcInf trayLogs, ref int errorno, ref string msg)
		{
			try
			{
				int result = 0;
				errorno = 0;
				msg = "";
				if (Setting.Value.plsdata.debug_1 == 1)
				{
					trayLogs.argument1 = "123";
					trayLogs.P_Id = "2";
				}
				switch (int.Parse(trayLogs.P_Id))
				{
				case 2:
					if (int.Parse(trayLogs.argument1) != 0)
					{
						errorno = 110001;
						msg = "原点復帰に失敗しました。";
					}
					break;
				case 4:
					errorno = 0;
					msg = "";
					break;
				case 5:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "ノズル先端検知に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 6:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "ノズル洗浄に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 7:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "2重管ノズル乾燥に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 8:
					if (int.Parse(trayLogs.argument9) != 0)
					{
						errorno = int.Parse(trayLogs.argument9);
					}
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "液面検知に失敗しました。";
					}
					break;
				case 9:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "試薬攪拌に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 10:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "ピペット試薬吸引に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 11:
					if (int.Parse(trayLogs.argument10) != 0)
					{
						errorno = int.Parse(trayLogs.argument10);
					}
					if (int.Parse(trayLogs.argument9) != 0)
					{
						errorno = int.Parse(trayLogs.argument9);
					}
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (errorno != 0)
					{
						msg = "試薬吐出に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 12:
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (int.Parse(trayLogs.argument2) != 0)
					{
						errorno = int.Parse(trayLogs.argument2);
					}
					if (errorno != 0)
					{
						msg = "試薬吐出後攪拌に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 13:
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (int.Parse(trayLogs.argument2) != 0)
					{
						errorno = int.Parse(trayLogs.argument2);
					}
					if (errorno != 0)
					{
						msg = "試薬廃液に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 14:
					if (int.Parse(trayLogs.argument11) != 0)
					{
						errorno = int.Parse(trayLogs.argument11);
					}
					if (int.Parse(trayLogs.argument10) != 0)
					{
						errorno = int.Parse(trayLogs.argument10);
					}
					if (int.Parse(trayLogs.argument9) != 0)
					{
						errorno = int.Parse(trayLogs.argument9);
					}
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (errorno != 0)
					{
						msg = "液溜め洗浄に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 15:
					if (int.Parse(trayLogs.argument11) != 0)
					{
						errorno = int.Parse(trayLogs.argument11);
					}
					if (int.Parse(trayLogs.argument10) != 0)
					{
						errorno = int.Parse(trayLogs.argument10);
					}
					if (int.Parse(trayLogs.argument9) != 0)
					{
						errorno = int.Parse(trayLogs.argument9);
					}
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (errorno != 0)
					{
						msg = "液溜め残液制御に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 16:
					if (int.Parse(trayLogs.argument9) != 0)
					{
						errorno = int.Parse(trayLogs.argument9);
					}
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (errorno != 0)
					{
						msg = "2重管ノズル配管充填に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 18:
					if (int.Parse(trayLogs.argument8) != 0)
					{
						errorno = int.Parse(trayLogs.argument8);
					}
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (errorno != 0)
					{
						msg = "2重管ノズル配管充填に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 20:
					if (int.Parse(trayLogs.argument7) != 0)
					{
						errorno = int.Parse(trayLogs.argument7);
					}
					if (int.Parse(trayLogs.argument6) != 0)
					{
						errorno = int.Parse(trayLogs.argument6);
					}
					if (int.Parse(trayLogs.argument5) != 0)
					{
						errorno = int.Parse(trayLogs.argument5);
					}
					if (int.Parse(trayLogs.argument4) != 0)
					{
						errorno = int.Parse(trayLogs.argument4);
					}
					if (int.Parse(trayLogs.argument3) != 0)
					{
						errorno = int.Parse(trayLogs.argument3);
					}
					if (int.Parse(trayLogs.argument2) != 0)
					{
						errorno = int.Parse(trayLogs.argument2);
					}
					if (errorno != 0)
					{
						msg = "試薬廃液に失敗しました。";
					}
					else
					{
						msg = "";
					}
					break;
				case 22:
					P_END = 1;
					break;
				case 23:
					S1_END = 1;
					break;
				}
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
				return -1;
			}
		}

		public int ReadQCLogqData(ref int errorno, ref string err_msg)
		{
			byte[] array = new byte[60];
			int w = 0;
			plcio plcio2 = new plcio();
			string text = "";
			PlcInf trayLogs = new PlcInf();
			try
			{
				GetSeqData(ref rcurno, ref w);
				if (Setting.Value.plsdata.debug_1 == 1)
				{
					rcurno = 0;
					w = 9;
				}
				if (GetLogqData(rcurno, ref trayLogs) == 0)
				{
					PlcAnaLogData(trayLogs, ref errorno, ref err_msg);
				}
				if (P_END == 1)
				{
					P_END = 0;
					Form1.protocol_status = 0;
					Form1.unit_status = 2;
					Form1.unit_status2 = 2;
					if (batchtrayLogs.Count != 0)
					{
						batchtrayLogs.Clear();
					}
				}
				return 0;
			}
			catch (Exception ex)
			{
				if (P_END == 1)
				{
					P_END = 0;
					Form1.protocol_status = 0;
					Form1.unit_status = 2;
					Form1.unit_status2 = 2;
					if (batchtrayLogs.Count != 0)
					{
						batchtrayLogs.Clear();
					}
				}
				Mult_Reagent1.Log.Log.Error("ReadQCLogqData エラー" + ex.ToString(), "ReadQCLogqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 714);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
			return 0;
		}

		public int ReadLogqData()
		{
			byte[] array = new byte[60];
			int num = 0;
			int w = 0;
			plcio plcio2 = new plcio();
			string path = "";
			PlcInf trayLogs = new PlcInf();
			try
			{
				GetSeqData(ref rcurno, ref w);
				if (Setting.Value.plsdata.debug_1 == 1)
				{
					rcurno = 5;
					w = 6;
				}
				while (rcurno != w)
				{
					if (GetLogqData(rcurno, ref trayLogs) == 0)
					{
						PlcAnaData(trayLogs, ref path);
					}
					string endDateTime = DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss");
					Form1.Loginf[0].EndDateTime = endDateTime;
					Form1.Loginf[1].EndDateTime = endDateTime;
					Form1.Loginf[2].EndDateTime = endDateTime;
					Form1.Loginf[3].EndDateTime = endDateTime;
					if (!Form1.isProcess3)
					{
						InfLog_Write(path);
					}
					if (S1_END == 1)
					{
						Form1.unit_status = 2;
					}
					if (P_END == 1)
					{
						P_END = 0;
						Form1.protocol_status = 0;
						Form1.protocol_ing = 0;
						Form1.unit_status = 2;
						Form1.unit_status2 = 2;
						if (batchtrayLogs.Count != 0)
						{
							batchtrayLogs.Clear();
						}
						break;
					}
				}
				return 0;
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("ReadLogqData エラー" + ex.ToString(), "ReadLogqData", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 814);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
				if (S1_END == 1)
				{
					Form1.unit_status = 2;
				}
				if (P_END == 1)
				{
					P_END = 0;
					Form1.protocol_status = 0;
					Form1.protocol_ing = 0;
					Form1.unit_status = 2;
					Form1.unit_status2 = 2;
					if (batchtrayLogs.Count != 0)
					{
						batchtrayLogs.Clear();
					}
				}
			}
			return 0;
		}

		private static string PlcAnaData(PlcInf trayLogs, ref string path)
		{
			int num = 0;
			try
			{
				string[] array = new string[24]
				{
					"0", "PVER", "MVIP", "LOPT", "DSPT", "SNPT", "NOWA", "NODR", "LDRG", "STRG",
					"SCRG", "DSRG", "SDRG", "DCRG", "WACP", "CRL1", "CRL2", "STMP", "DSPM", "WAIT",
					"SPAR", "PCNT", "PEND", "S1FN"
				};
				string text = DateTime.Now.ToString("yyyyMMdd");
				string path2 = Setting.Value.File_Path.InerLogPath + "\\" + text;
				if (!Directory.Exists(path2))
				{
					Directory.CreateDirectory(path2);
				}
				path2 = ((!Form1.isProcess3) ? (Setting.Value.File_Path.InerLogPath + "\\" + text + "\\InerLog.csv") : (Setting.Value.File_Path.InerLogPath + "\\" + text + "\\QcInerLog.csv"));
				num = int.Parse(trayLogs.P_Id);
				trayLogs.P_Id = array[num];
				if (File.Exists(path2))
				{
					Makecsv(trayLogs, path2);
					Console.WriteLine("存在します");
				}
				else
				{
					ExportToCsv(trayLogs, path2);
					Console.WriteLine("存在しません");
				}
				return "";
			}
			catch (Exception ex)
			{
				Console.WriteLine("PlcAnaData エラーが発生しました: " + ex.ToString());
				return "";
			}
		}

		private static void ExportToCsv(PlcInf trayLogs, string outputPath)
		{
			string value = "P_Id,S_date,S_Time,E_date,E_Time,argument1,argument2,argument3,argument4,argument5,argument6,argument7,argument8,argument9,argument10,argument11,argument12,argument13,argument14,argument15,argument16,argument17";
			try
			{
				using StreamWriter streamWriter = new StreamWriter(outputPath);
				streamWriter.WriteLine(value);
				string value2 = trayLogs.P_Id + "," + trayLogs.S_date + "," + trayLogs.S_Time + "," + trayLogs.E_date + "," + trayLogs.E_Time + "," + trayLogs.argument1 + "," + trayLogs.argument2 + "," + trayLogs.argument3 + "," + trayLogs.argument4 + "," + trayLogs.argument5 + "," + trayLogs.argument6 + "," + trayLogs.argument7 + "," + trayLogs.argument8 + "," + trayLogs.argument9 + "," + trayLogs.argument10 + "," + trayLogs.argument11 + "," + trayLogs.argument12 + "," + trayLogs.argument13 + "," + trayLogs.argument14 + "," + trayLogs.argument15 + "," + trayLogs.argument16 + "," + trayLogs.argument17;
				streamWriter.WriteLine(value2);
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("ExportToCsv エラー" + ex.ToString(), "ExportToCsv", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 945);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
		}

		private static int Makecsv(PlcInf trayLogs, string outputPat)
		{
			string contents = trayLogs.P_Id + "," + trayLogs.S_date + "," + trayLogs.S_Time + "," + trayLogs.E_date + "," + trayLogs.E_Time + "," + trayLogs.argument1 + "," + trayLogs.argument2 + "," + trayLogs.argument3 + "," + trayLogs.argument4 + "," + trayLogs.argument5 + "," + trayLogs.argument6 + "," + trayLogs.argument7 + "," + trayLogs.argument8 + "," + trayLogs.argument9 + "," + trayLogs.argument10 + "," + trayLogs.argument11 + "," + trayLogs.argument12 + "," + trayLogs.argument13 + "," + trayLogs.argument14 + "," + trayLogs.argument15 + "," + trayLogs.argument16 + "," + trayLogs.argument17 + ",\n";
			File.AppendAllText(outputPat, contents);
			return 0;
		}

		public void InfLog_Write(string path)
		{
			try
			{
				int num = 0;
				string text = "";
				string text2 = DateTime.Now.ToString("yyyyMMdd");
				text = Setting.Value.File_Path.LogPath;
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				num = Setting.Value.Machin_type.machine_no;
				text = ((Setting.Value.Machin_type.m_type != 1) ? (Setting.Value.File_Path.LogPath + "\\") : (Setting.Value.File_Path.LogPath + "\\"));
				ExportToCsv_inftray(text);
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("InfLog_Write エラー" + ex.ToString(), "InfLog_Write", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 1017);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
		}

		private static int Protocol_Error_Anaalize(int no, ref string ErrorCode, ref string ErrorDateTime)
		{
			ErrorCode = "";
			ErrorDateTime = "";
			return 0;
		}

		private static void ExportToCsv_inftray(string outputPath)
		{
			int num = 0;
			string value = "RecTrayBarcode,StartDateTime,EndDateTime,PrismFileName,ErrorCode,ErrorDateTime,ErrorDetail,UnitCode,UnitNumber,UserProtocolName1,UserProtocolName2,ReagentName1,ReagentName2,ReagentName3,ReagentName4,ReagentName5,ReagentName6,ReagentName7,ReagentName8,ClaningLiquidName1,ClaningLiquidName2,ClaningLiquidName3,ClaningLiquidName4";
			string value2 = "PrismNo,Normal/Reference,ProcessStart,ProcessEnd,ErrorCode,ErrorDateTime,ErrorDetail";
			string text = "";
			string text2 = DateTime.Now.ToString("yyyyMMdd");
			try
			{
				int num2 = 0;
				string no = "";
				string emsg = "";
				num2 = 0;
				text = ((Setting.Value.Machin_type.m_type != 1) ? (outputPath + "\\" + text2 + "_4" + Setting.Value.Machin_type.machine_no + ".csv") : (outputPath + "\\" + text2 + "_2" + Setting.Value.Machin_type.machine_no + ".csv"));
				num = 0;
				if (File.Exists(text))
				{
					num = 1;
				}
				Encoding encoding = Encoding.GetEncoding("UTF-8");
				if (num == 0)
				{
					using StreamWriter streamWriter = new StreamWriter(text, append: false, encoding);
					streamWriter.WriteLine(value);
					if (Form1.Loginf[0].RecTrayBarcode != "" && Form1.Loginf[0].RecTrayBarcode != null)
					{
						if (Form1.Loginf[0].ErrorCode == "0" && Form1.Loginf[0].ErrorCode == null)
						{
							Form1.Loginf[0].ErrorCode = "0";
						}
						string value3 = Form1.Loginf[0].RecTrayBarcode + "," + Form1.Loginf[0].StartDateTime + "," + Form1.Loginf[0].EndDateTime + "," + Form1.Loginf[0].PrismFileName + "," + Form1.Loginf[0].ErrorCode + "," + Form1.Loginf[0].ErrorDateTime + "," + Form1.Loginf[0].ErrorDetail + "," + Form1.Loginf[0].UnitCode + "," + Form1.Loginf[0].UnitNumber + "," + Form1.Loginf[0].UserProtocolName1 + "," + Form1.Loginf[0].UserProtocolName2 + "," + Form1.Loginf[0].ReagentName1 + "," + Form1.Loginf[0].ReagentName2 + "," + Form1.Loginf[0].ReagentName3 + "," + Form1.Loginf[0].ReagentName4 + "," + Form1.Loginf[0].ReagentName5 + "," + Form1.Loginf[0].ReagentName6 + "," + Form1.Loginf[0].ReagentName7 + "," + Form1.Loginf[0].ReagentName8 + "," + Form1.Loginf[0].ClaningLiquidName1 + "," + Form1.Loginf[0].ClaningLiquidName2 + "," + Form1.Loginf[0].ClaningLiquidName3 + "," + Form1.Loginf[0].ClaningLiquidName4;
						streamWriter.WriteLine(value3);
					}
					if (Form1.Loginf[1].RecTrayBarcode != "" && Form1.Loginf[1].RecTrayBarcode != null)
					{
						if (Form1.Loginf[1].ErrorCode == "0" && Form1.Loginf[1].ErrorCode == null)
						{
							Form1.Loginf[1].ErrorCode = "0";
						}
						string value4 = Form1.Loginf[1].RecTrayBarcode + "," + Form1.Loginf[1].StartDateTime + "," + Form1.Loginf[1].EndDateTime + "," + Form1.Loginf[1].PrismFileName + "," + Form1.Loginf[1].ErrorCode + "," + Form1.Loginf[1].ErrorDateTime + "," + Form1.Loginf[1].ErrorDetail + "," + Form1.Loginf[1].UnitCode + "," + Form1.Loginf[1].UnitNumber + "," + Form1.Loginf[1].UserProtocolName1 + "," + Form1.Loginf[1].UserProtocolName2 + "," + Form1.Loginf[1].ReagentName1 + "," + Form1.Loginf[1].ReagentName2 + "," + Form1.Loginf[1].ReagentName3 + "," + Form1.Loginf[1].ReagentName4 + "," + Form1.Loginf[1].ReagentName5 + "," + Form1.Loginf[1].ReagentName6 + "," + Form1.Loginf[1].ReagentName7 + "," + Form1.Loginf[1].ReagentName8 + "," + Form1.Loginf[1].ClaningLiquidName1 + "," + Form1.Loginf[1].ClaningLiquidName2 + "," + Form1.Loginf[1].ClaningLiquidName3 + "," + Form1.Loginf[1].ClaningLiquidName4;
						streamWriter.WriteLine(value4);
					}
					if (Form1.Loginf[2].RecTrayBarcode != "" && Form1.Loginf[2].RecTrayBarcode != null)
					{
						if (Form1.Loginf[2].ErrorCode == "0" && Form1.Loginf[2].ErrorCode == null)
						{
							Form1.Loginf[2].ErrorCode = "0";
						}
						string value5 = Form1.Loginf[2].RecTrayBarcode + "," + Form1.Loginf[2].StartDateTime + "," + Form1.Loginf[2].EndDateTime + "," + Form1.Loginf[2].PrismFileName + "," + Form1.Loginf[2].ErrorCode + "," + Form1.Loginf[2].ErrorDateTime + "," + Form1.Loginf[2].ErrorDetail + "," + Form1.Loginf[2].UnitCode + "," + Form1.Loginf[2].UnitNumber + "," + Form1.Loginf[2].UserProtocolName1 + "," + Form1.Loginf[2].UserProtocolName2 + "," + Form1.Loginf[2].ReagentName1 + "," + Form1.Loginf[2].ReagentName2 + "," + Form1.Loginf[2].ReagentName3 + "," + Form1.Loginf[2].ReagentName4 + "," + Form1.Loginf[2].ReagentName5 + "," + Form1.Loginf[2].ReagentName6 + "," + Form1.Loginf[2].ReagentName7 + "," + Form1.Loginf[2].ReagentName8 + "," + Form1.Loginf[2].ClaningLiquidName1 + "," + Form1.Loginf[2].ClaningLiquidName2 + "," + Form1.Loginf[2].ClaningLiquidName3 + "," + Form1.Loginf[2].ClaningLiquidName4;
						streamWriter.WriteLine(value5);
					}
					if (Form1.Loginf[3].RecTrayBarcode != "" && Form1.Loginf[3].RecTrayBarcode != null)
					{
						if (Form1.Loginf[3].ErrorCode == "0" && Form1.Loginf[3].ErrorCode == null)
						{
							Form1.Loginf[3].ErrorCode = "0";
						}
						string value6 = Form1.Loginf[3].RecTrayBarcode + "," + Form1.Loginf[3].StartDateTime + "," + Form1.Loginf[3].EndDateTime + "," + Form1.Loginf[3].PrismFileName + "," + Form1.Loginf[3].ErrorCode + "," + Form1.Loginf[3].ErrorDateTime + "," + Form1.Loginf[3].ErrorDetail + "," + Form1.Loginf[3].UnitCode + "," + Form1.Loginf[3].UnitNumber + "," + Form1.Loginf[3].UserProtocolName1 + "," + Form1.Loginf[3].UserProtocolName2 + "," + Form1.Loginf[3].ReagentName1 + "," + Form1.Loginf[3].ReagentName2 + "," + Form1.Loginf[3].ReagentName3 + "," + Form1.Loginf[3].ReagentName4 + "," + Form1.Loginf[3].ReagentName5 + "," + Form1.Loginf[3].ReagentName6 + "," + Form1.Loginf[3].ReagentName7 + "," + Form1.Loginf[3].ReagentName8 + "," + Form1.Loginf[3].ClaningLiquidName1 + "," + Form1.Loginf[3].ClaningLiquidName2 + "," + Form1.Loginf[3].ClaningLiquidName3 + "," + Form1.Loginf[3].ClaningLiquidName4;
						streamWriter.WriteLine(value6);
					}
				}
				else
				{
					string text3 = DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss");
					for (int i = 0; i < 4; i++)
					{
						if (!(Form1.Loginf[i].RecTrayBarcode != ""))
						{
							continue;
						}
						List<string> list = File.ReadAllLines(text).ToList();
						for (int j = 0; j < list.Count; j++)
						{
							string[] array = list[j].Split(',');
							if (!(array[0] == Form1.Loginf[i].RecTrayBarcode))
							{
								continue;
							}
							array[1] = Form1.Loginf[i].StartDateTime;
							array[2] = text3;
							if (Form1.Loginf[i].ErrorCode != "")
							{
								array[3] = Form1.Loginf[i].PrismFileName;
								if (Setting.Value.Machin_type.m_type == 1)
								{
									array[9] = Form1.Loginf[i].UserProtocolName1;
								}
								else
								{
									array[10] = Form1.Loginf[i].UserProtocolName1;
								}
								array[11] = Form1.Loginf[i].ReagentName1;
								array[12] = Form1.Loginf[i].ReagentName2;
								array[13] = Form1.Loginf[i].ReagentName3;
								array[14] = Form1.Loginf[i].ReagentName4;
								array[15] = Form1.Loginf[i].ReagentName5;
								array[16] = Form1.Loginf[i].ReagentName6;
								array[17] = Form1.Loginf[i].ReagentName7;
								array[18] = Form1.Loginf[i].ReagentName8;
								array[19] = Form1.Loginf[i].ClaningLiquidName1;
								array[20] = Form1.Loginf[i].ClaningLiquidName2;
								array[21] = Form1.Loginf[i].ClaningLiquidName3;
								array[22] = Form1.Loginf[i].ClaningLiquidName4;
								list[j] = string.Join(",", array);
								continue;
							}
							array[3] = "0";
							string ErrorCode = Form1.Loginf[i].ErrorCode;
							string ErrorDateTime = "";
							Protocol_Error_Anaalize(i, ref ErrorCode, ref ErrorDateTime);
							array[3] = ErrorCode;
							array[4] = Form1.Loginf[i].ErrorDateTime;
							array[5] = ErrorDateTime;
							if (Setting.Value.Machin_type.m_type == 1)
							{
								array[9] = Form1.Loginf[i].UserProtocolName1;
							}
							else
							{
								array[10] = Form1.Loginf[i].UserProtocolName1;
							}
							array[11] = Form1.Loginf[i].ReagentName1;
							array[12] = Form1.Loginf[i].ReagentName2;
							array[13] = Form1.Loginf[i].ReagentName3;
							array[14] = Form1.Loginf[i].ReagentName4;
							array[15] = Form1.Loginf[i].ReagentName5;
							array[16] = Form1.Loginf[i].ReagentName6;
							array[17] = Form1.Loginf[i].ReagentName7;
							array[18] = Form1.Loginf[i].ReagentName8;
							array[19] = Form1.Loginf[i].ClaningLiquidName1;
							array[20] = Form1.Loginf[i].ClaningLiquidName2;
							array[21] = Form1.Loginf[i].ClaningLiquidName3;
							array[22] = Form1.Loginf[i].ClaningLiquidName4;
							list[j] = string.Join(",", array);
						}
						File.WriteAllLines(text, list, Encoding.UTF8);
						list.Clear();
					}
					int num3 = 0;
					for (int k = 0; k < 4; k++)
					{
						num3 = 0;
						if (!(Form1.Loginf[k].RecTrayBarcode != ""))
						{
							continue;
						}
						List<string> list2 = File.ReadAllLines(text).ToList();
						for (int l = 1; l < list2.Count; l++)
						{
							string[] array2 = list2[l].Split(',');
							if (array2[0] == Form1.Loginf[k].RecTrayBarcode)
							{
								num3 = 1;
							}
						}
						if (num3 == 0 && Form1.Loginf[k].RecTrayBarcode != "" && Form1.Loginf[k].RecTrayBarcode != null && Form1.Loginf[k].RecTrayBarcode != "" && Form1.Loginf[k].RecTrayBarcode != null)
						{
							if (Form1.Loginf[k].ErrorCode == "0" && Form1.Loginf[k].ErrorCode == null)
							{
								Form1.Loginf[k].ErrorCode = "0";
								string contents = Form1.Loginf[k].RecTrayBarcode + "," + Form1.Loginf[k].StartDateTime + "," + Form1.Loginf[k].EndDateTime + "," + Form1.Loginf[k].PrismFileName + "," + Form1.Loginf[k].ErrorCode + "," + Form1.Loginf[k].ErrorDateTime + "," + Form1.Loginf[k].ErrorDetail + "," + Form1.Loginf[k].UnitCode + "," + Form1.Loginf[k].UnitNumber + "," + Form1.Loginf[k].UserProtocolName1 + "," + Form1.Loginf[k].UserProtocolName2 + "," + Form1.Loginf[k].ReagentName1 + "," + Form1.Loginf[k].ReagentName2 + "," + Form1.Loginf[k].ReagentName3 + "," + Form1.Loginf[k].ReagentName4 + "," + Form1.Loginf[k].ReagentName5 + "," + Form1.Loginf[k].ReagentName6 + "," + Form1.Loginf[k].ReagentName7 + "," + Form1.Loginf[k].ReagentName8 + "," + Form1.Loginf[k].ClaningLiquidName1 + "," + Form1.Loginf[k].ClaningLiquidName2 + "," + Form1.Loginf[k].ClaningLiquidName3 + "," + Form1.Loginf[k].ClaningLiquidName4 + ",\n";
								File.AppendAllText(text, contents, Encoding.UTF8);
							}
							else
							{
								string contents2 = Form1.Loginf[k].RecTrayBarcode + "," + Form1.Loginf[k].StartDateTime + "," + Form1.Loginf[k].EndDateTime + "," + Form1.Loginf[k].PrismFileName + "," + Form1.Loginf[k].ErrorCode + "," + Form1.Loginf[k].ErrorDateTime + "," + Form1.Loginf[k].ErrorDetail + "," + Form1.Loginf[k].UnitCode + "," + Form1.Loginf[k].UnitNumber + "," + Form1.Loginf[k].UserProtocolName1 + "," + Form1.Loginf[k].UserProtocolName2 + "," + Form1.Loginf[k].ReagentName1 + "," + Form1.Loginf[k].ReagentName2 + "," + Form1.Loginf[k].ReagentName3 + "," + Form1.Loginf[k].ReagentName4 + "," + Form1.Loginf[k].ReagentName5 + "," + Form1.Loginf[k].ReagentName6 + "," + Form1.Loginf[k].ReagentName7 + "," + Form1.Loginf[k].ReagentName8 + "," + Form1.Loginf[k].ClaningLiquidName1 + "," + Form1.Loginf[k].ClaningLiquidName2 + "," + Form1.Loginf[k].ClaningLiquidName3 + "," + Form1.Loginf[k].ClaningLiquidName4 + ",\n";
								File.AppendAllText(text, contents2, Encoding.UTF8);
							}
						}
						list2.Clear();
					}
				}
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				if (Setting.Value.Machin_type.m_type != 1)
				{
					num2 = 1;
				}
				for (int m = 0; m < 4; m++)
				{
					if (Form1.Loginf[m].RecTrayBarcode != "" && Form1.Loginf[m].RecTrayBarcode != null)
					{
						num2++;
						if (Setting.Value.Machin_type.m_type != 1)
						{
							num2 = 1;
						}
					}
				}
				for (int n = 0; n < batchtrayLogs.Count; n++)
				{
					num8 = 0;
					if (Setting.Value.plsdata.debug_1 == 1)
					{
						batchtrayLogs[n].P_Id = "DSRG";
					}
					if (batchtrayLogs[n].P_Id == "DSRG")
					{
						num8 = 1;
					}
					if (batchtrayLogs[n].P_Id == "WACP")
					{
						num8 = 2;
					}
					if (batchtrayLogs[n].P_Id == "CRL1")
					{
						num8 = 3;
					}
					if (Setting.Value.plsdata.debug_1 == 1)
					{
						batchtrayLogs[n].argument6 = "1";
						num8 = 1;
					}
					if (num8 == 0)
					{
						continue;
					}
					num5 = int.Parse(batchtrayLogs[n].argument2);
					num7 = int.Parse(batchtrayLogs[n].argument4);
					if (Setting.Value.plsdata.debug_1 == 1)
					{
						num5 = 2;
						num7 = 4;
					}
					if (num5 == 0 || num7 == 0)
					{
						continue;
					}
					num5--;
					num6 = (num7 - 1) * 6;
					if (num5 < 0)
					{
						continue;
					}
					if (int.Parse(batchtrayLogs[n].argument5) == 0)
					{
						Form1.PrismLog[num5][num6].ErrorCode = "0";
						Form1.PrismLog[num5][num6].ErrorDetail = "";
					}
					else if (num8 == 1)
					{
						if (num8 == 1)
						{
							num4 = 1;
						}
						Form1.PrismLog[num5][num6].ErrorCode = no;
						Form1.PrismLog[num5][num6].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					else
					{
						Form1.PrismLog[num5][num6].ErrorCode = "0";
						Form1.PrismLog[num5][num6].ErrorDetail = "";
					}
					Form1.PrismLog[num5][num6].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6].ProcessEnd = Form1.Loginf[num5].EndDateTime;
					if (int.Parse(batchtrayLogs[n].argument6) == 0)
					{
						Form1.PrismLog[num5][num6 + 1].ErrorCode = "0";
						Form1.PrismLog[num5][num6 + 1].ErrorDetail = "";
					}
					else
					{
						if (num8 == 1)
						{
							Form1.GetErrorMsg(102201, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 2)
						{
							Form1.GetErrorMsg(102104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 3)
						{
							Form1.GetErrorMsg(102104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						Form1.PrismLog[num5][num6 + 1].ErrorCode = no;
						Form1.PrismLog[num5][num6 + 1].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6 + 1].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					Form1.PrismLog[num5][num6 + 1].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6 + 1].ProcessEnd = Form1.Loginf[num5].EndDateTime;
					if (int.Parse(batchtrayLogs[n].argument7) == 0)
					{
						Form1.PrismLog[num5][num6 + 2].ErrorCode = "0";
						Form1.PrismLog[num5][num6 + 2].ErrorDetail = "";
					}
					else
					{
						if (num8 == 1)
						{
							Form1.GetErrorMsg(103201, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 2)
						{
							Form1.GetErrorMsg(103104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 3)
						{
							Form1.GetErrorMsg(103104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						Form1.PrismLog[num5][num6 + 2].ErrorCode = no;
						Form1.PrismLog[num5][num6 + 2].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6 + 2].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					Form1.PrismLog[num5][num6 + 2].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6 + 2].ProcessEnd = Form1.Loginf[num5].EndDateTime;
					if (int.Parse(batchtrayLogs[n].argument8) == 0)
					{
						Form1.PrismLog[num5][num6 + 3].ErrorCode = "0";
						Form1.PrismLog[num5][num6 + 3].ErrorDetail = "";
					}
					else
					{
						if (num8 == 1)
						{
							Form1.GetErrorMsg(104201, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 2)
						{
							Form1.GetErrorMsg(104104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 3)
						{
							Form1.GetErrorMsg(104104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						Form1.PrismLog[num5][num6 + 3].ErrorCode = no;
						Form1.PrismLog[num5][num6 + 3].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6 + 3].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					Form1.PrismLog[num5][num6 + 3].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6 + 3].ProcessEnd = Form1.Loginf[num5].EndDateTime;
					if (int.Parse(batchtrayLogs[n].argument9) == 0)
					{
						Form1.PrismLog[num5][num6 + 4].ErrorCode = "0";
						Form1.PrismLog[num5][num6 + 4].ErrorDetail = "";
					}
					else
					{
						if (num8 == 1)
						{
							Form1.GetErrorMsg(105201, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 2)
						{
							Form1.GetErrorMsg(105104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 3)
						{
							Form1.GetErrorMsg(105104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						Form1.PrismLog[num5][num6 + 4].ErrorCode = no;
						Form1.PrismLog[num5][num6 + 4].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6 + 4].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					Form1.PrismLog[num5][num6 + 4].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6 + 4].ProcessEnd = Form1.Loginf[num5].EndDateTime;
					if (int.Parse(batchtrayLogs[n].argument10) == 0)
					{
						Form1.PrismLog[num5][num6 + 5].ErrorCode = "0";
						Form1.PrismLog[num5][num6 + 5].ErrorDetail = "";
					}
					else
					{
						if (num8 == 1)
						{
							Form1.GetErrorMsg(106201, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 2)
						{
							Form1.GetErrorMsg(106104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						if (num8 == 3)
						{
							Form1.GetErrorMsg(106104, ref no, ref emsg);
							if (num4 == 0)
							{
								num4 = 1;
							}
						}
						Form1.PrismLog[num5][num6 + 5].ErrorCode = no;
						Form1.PrismLog[num5][num6 + 5].ErrorDetail = emsg;
						Form1.PrismLog[num5][num6 + 5].ErrorDateTime = batchtrayLogs[n].E_date + "_" + batchtrayLogs[n].E_Time;
					}
					Form1.PrismLog[num5][num6 + 5].ProcessStart = Form1.Loginf[num5].StartDateTime;
					Form1.PrismLog[num5][num6 + 5].ProcessEnd = Form1.Loginf[num5].EndDateTime;
				}
				string text4 = "";
				for (int num9 = 0; num9 < num2; num9++)
				{
					text = outputPath + "\\" + Form1.Loginf[num9].PrismFileName;
					if (Form1.Loginf[num9].RecTrayBarcode == "" && Form1.Loginf[num9].RecTrayBarcode == null)
					{
						continue;
					}
					if (File.Exists(text))
					{
						using (FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read))
						{
							fileStream.Dispose();
						}
						File.Delete(text);
					}
					encoding = Encoding.GetEncoding("shift-jis");
					using StreamWriter streamWriter2 = new StreamWriter(text, append: false, encoding);
					streamWriter2.WriteLine(value2);
					int num10 = 0;
					for (int num11 = 0; num11 < 4; num11++)
					{
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
						text4 = Form1.PrismLog[num9][num10].PrismNo + "," + Form1.PrismLog[num9][num10].NormalReference + "," + Form1.PrismLog[num9][num10].ProcessStart + "," + Form1.PrismLog[num9][num10].ProcessEnd + "," + Form1.PrismLog[num9][num10].ErrorCode + "," + Form1.PrismLog[num9][num10].ErrorDateTime + "," + Form1.PrismLog[num9][num10].ErrorDetail + ",";
						streamWriter2.WriteLine(text4);
						num10++;
					}
				}
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("ExportToCsv_inftray エラー" + ex.ToString(), "ExportToCsv_inftray", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 1599);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
		}

		private static int Makecsv_inftray(string outputPath)
		{
			string text = "";
			try
			{
				int num = 0;
				string text2 = DateTime.Now.ToString("yyyyMMdd");
				text = ((Setting.Value.Machin_type.m_type != 1) ? (outputPath + "\\" + text2 + "_4" + Setting.Value.Machin_type.machine_no + ".csv") : (outputPath + "\\" + text2 + "_2" + Setting.Value.Machin_type.machine_no + ".csv"));
				using (new StreamWriter(text))
				{
					if (Setting.Value.Machin_type.m_type == 1)
					{
						string contents = Form1.Loginf[0].RecTrayBarcode + "," + Form1.Loginf[0].StartDateTime + "," + Form1.Loginf[0].EndDateTime + "," + Form1.Loginf[0].PrismFileName + "," + Form1.Loginf[0].ErrorCode + "," + Form1.Loginf[0].ErrorDateTime + "," + Form1.Loginf[0].UnitCode + "," + Form1.Loginf[0].UnitNumber + "," + Form1.Loginf[0].UserProtocolName1 + "," + Form1.Loginf[0].UserProtocolName2 + "," + Form1.Loginf[0].ReagentName1 + "," + Form1.Loginf[0].ReagentName2 + "," + Form1.Loginf[0].ReagentName3 + "," + Form1.Loginf[0].ReagentName4 + "," + Form1.Loginf[0].ReagentName5 + "," + Form1.Loginf[0].ReagentName7 + "," + Form1.Loginf[0].ReagentName8 + "," + Form1.Loginf[0].ClaningLiquidName1 + "," + Form1.Loginf[0].ClaningLiquidName2 + "," + Form1.Loginf[0].ClaningLiquidName3 + "," + Form1.Loginf[0].ClaningLiquidName4 + ",\n";
						string text3 = Form1.Loginf[1].RecTrayBarcode + "," + Form1.Loginf[1].StartDateTime + "," + Form1.Loginf[1].EndDateTime + "," + Form1.Loginf[1].PrismFileName + "," + Form1.Loginf[1].ErrorCode + "," + Form1.Loginf[1].ErrorDateTime + "," + Form1.Loginf[1].UnitCode + "," + Form1.Loginf[1].UnitNumber + "," + Form1.Loginf[1].UserProtocolName1 + "," + Form1.Loginf[1].UserProtocolName2 + "," + Form1.Loginf[1].ReagentName1 + "," + Form1.Loginf[1].ReagentName2 + "," + Form1.Loginf[1].ReagentName3 + "," + Form1.Loginf[1].ReagentName4 + "," + Form1.Loginf[1].ReagentName5 + "," + Form1.Loginf[1].ReagentName7 + "," + Form1.Loginf[1].ReagentName8 + "," + Form1.Loginf[1].ClaningLiquidName1 + "," + Form1.Loginf[1].ClaningLiquidName2 + "," + Form1.Loginf[1].ClaningLiquidName3 + "," + Form1.Loginf[1].ClaningLiquidName4 + ",\n";
						string contents2 = Form1.Loginf[2].RecTrayBarcode + "," + Form1.Loginf[2].StartDateTime + "," + Form1.Loginf[2].EndDateTime + "," + Form1.Loginf[2].PrismFileName + "," + Form1.Loginf[2].ErrorCode + "," + Form1.Loginf[2].ErrorDateTime + "," + Form1.Loginf[2].UnitCode + "," + Form1.Loginf[2].UnitNumber + "," + Form1.Loginf[2].UserProtocolName1 + "," + Form1.Loginf[2].UserProtocolName2 + "," + Form1.Loginf[2].ReagentName1 + "," + Form1.Loginf[2].ReagentName2 + "," + Form1.Loginf[2].ReagentName3 + "," + Form1.Loginf[2].ReagentName4 + "," + Form1.Loginf[2].ReagentName5 + "," + Form1.Loginf[2].ReagentName7 + "," + Form1.Loginf[2].ReagentName8 + "," + Form1.Loginf[2].ClaningLiquidName1 + "," + Form1.Loginf[2].ClaningLiquidName2 + "," + Form1.Loginf[2].ClaningLiquidName3 + "," + Form1.Loginf[2].ClaningLiquidName4 + ",\n";
						string contents3 = Form1.Loginf[3].RecTrayBarcode + "," + Form1.Loginf[3].StartDateTime + "," + Form1.Loginf[3].EndDateTime + "," + Form1.Loginf[3].PrismFileName + "," + Form1.Loginf[3].ErrorCode + "," + Form1.Loginf[3].ErrorDateTime + "," + Form1.Loginf[3].UnitCode + "," + Form1.Loginf[3].UnitNumber + "," + Form1.Loginf[3].UserProtocolName1 + "," + Form1.Loginf[3].UserProtocolName2 + "," + Form1.Loginf[3].ReagentName1 + "," + Form1.Loginf[3].ReagentName2 + "," + Form1.Loginf[3].ReagentName3 + "," + Form1.Loginf[3].ReagentName4 + "," + Form1.Loginf[3].ReagentName5 + "," + Form1.Loginf[3].ReagentName7 + "," + Form1.Loginf[3].ReagentName8 + "," + Form1.Loginf[3].ClaningLiquidName1 + "," + Form1.Loginf[3].ClaningLiquidName2 + "," + Form1.Loginf[3].ClaningLiquidName3 + "," + Form1.Loginf[3].ClaningLiquidName4 + ",\n";
						File.AppendAllText(text, contents);
						File.AppendAllText(text, contents);
						File.AppendAllText(text, contents2);
						File.AppendAllText(text, contents3);
					}
					else
					{
						string contents4 = Form1.Loginf[0].RecTrayBarcode + "," + Form1.Loginf[0].StartDateTime + "," + Form1.Loginf[0].EndDateTime + "," + Form1.Loginf[0].PrismFileName + "," + Form1.Loginf[0].ErrorCode + "," + Form1.Loginf[0].ErrorDateTime + "," + Form1.Loginf[0].UnitCode + "," + Form1.Loginf[0].UnitNumber + "," + Form1.Loginf[0].UserProtocolName1 + "," + Form1.Loginf[0].UserProtocolName2 + "," + Form1.Loginf[0].ReagentName1 + "," + Form1.Loginf[0].ReagentName2 + "," + Form1.Loginf[0].ReagentName3 + "," + Form1.Loginf[0].ReagentName4 + "," + Form1.Loginf[0].ReagentName5 + "," + Form1.Loginf[0].ReagentName7 + "," + Form1.Loginf[0].ReagentName8 + "," + Form1.Loginf[0].ClaningLiquidName1 + "," + Form1.Loginf[0].ClaningLiquidName2 + "," + Form1.Loginf[0].ClaningLiquidName3 + "," + Form1.Loginf[0].ClaningLiquidName4 + ",\n";
						File.AppendAllText(text, contents4);
					}
				}
				num = ((Setting.Value.Machin_type.m_type != 1) ? 1 : 4);
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < num; i++)
				{
					text = outputPath + "\\" + Form1.Loginf[i].PrismFileName;
					text = outputPath + "\\" + Form1.Loginf[i].PrismFileName;
					string text4 = "";
					using (new StreamWriter(text))
					{
						for (int j = 0; j < 4; j++)
						{
							for (int k = 0; k < batchtrayLogs.Count; k++)
							{
								if (int.Parse(batchtrayLogs[k].P_Id) != 11)
								{
									continue;
								}
								num2 = int.Parse(batchtrayLogs[k].argument2);
								if (num2 != i + 1)
								{
									continue;
								}
								string no = "";
								string emsg = "";
								num3 = int.Parse(batchtrayLogs[k].argument4);
								if (num3 == k)
								{
									if (int.Parse(batchtrayLogs[k].argument5) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref emsg);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
									if (int.Parse(batchtrayLogs[k].argument6) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref no);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
									if (int.Parse(batchtrayLogs[k].argument7) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref no);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
									if (int.Parse(batchtrayLogs[k].argument8) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref no);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
									if (int.Parse(batchtrayLogs[k].argument9) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref no);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
									if (int.Parse(batchtrayLogs[k].argument10) == 0)
									{
										Form1.PrismLog[i][j].ErrorCode = "0";
										Form1.PrismLog[i][j].ErrorDetail = "";
									}
									else
									{
										Form1.GetErrorMsg(101201, ref no, ref no);
										Form1.PrismLog[i][j].ErrorCode = no;
										Form1.PrismLog[i][j].ErrorDetail = emsg;
									}
									text4 = Form1.PrismLog[i][j].PrismNo + "," + Form1.PrismLog[i][j].NormalReference + "," + Form1.PrismLog[i][j].ProcessStart + "," + Form1.PrismLog[i][j].ProcessEnd + "," + Form1.PrismLog[i][j].ErrorCode + "," + Form1.PrismLog[i][j].ErrorDateTime + "," + Form1.PrismLog[i][j].ErrorDetail + ",\n";
									File.AppendAllText(text, text4);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Mult_Reagent1.Log.Log.Error("Makecsv_inftray エラー" + ex.ToString(), "Makecsv_inftray", "C:\\job\\スタック\\開発\\old\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio Log.cs", 1799);
				Console.WriteLine("エラーが発生しました: " + ex.ToString());
			}
			return 0;
		}
	}
}

 */