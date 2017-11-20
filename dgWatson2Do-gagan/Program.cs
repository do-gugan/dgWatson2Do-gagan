using System;
using System.IO;
using System.Linq;

using System.Text;
using System.Xml;
using System.Collections.Generic;
using Codeplex.Data;         //DynamicJson

namespace dgWatson2Dogagan
{
    class MainClass
    {
        public static string SaveText; //最終的にファイルに書き出す文字列

        public static void PrintText(double Start, string Speaker, string Transcript)
        {
            if (Transcript != "")
            {
                string NewLine = Start + "\t" + Speaker + "「" + Transcript + "」";
                Console.WriteLine(NewLine);
                SaveText += NewLine + "\n";
            }
        }

        public static void Main(string[] args) //起動オプションで渡されたファイル名がargsに入る（複数可）
        {
            if (args.Length == 0)
            {
                Console.WriteLine("読み込むファイルが指定されていません。");
            }
            else
            {
                foreach (string arg in args)
                    //渡されたファイル名ひとつずつに対して実行
                    ProcessSingleFile(arg);
            }

            //引数で渡されたファイル名を1つずつ処理する（メインループ）
            void ProcessSingleFile(string filename)
            {
                Console.WriteLine(filename + "を処理中...");

                var WatsonRes = DynamicJson.Parse(File.ReadAllText(@filename));
                //話者調査用ディクショナリを作成（開始タイムコードと話者コードのペア
                var speaker = new Dictionary<double, string>();
                //JSON後半にあるspeaker_labelsの各セットを列挙
                foreach (var sl in WatsonRes.speaker_labels)
                {
                    //fromフィールドとspeakerフィールドを抜き出してディクショナリに
                    speaker[sl.from] = sl.speaker.ToString();   
                }
                //以降、speaker[18.78]のように参照すれば、話者コードとして0などが返る

                String body = "";
                foreach (var result in WatsonRes.results)
                {
                    string CurrentSpeaker = "0";
                    double AltStart = 0;
                    string Transcript = "";
                    body += "\nalt start\n";

                    foreach (var alt in result.alternatives) //alternatives列挙
                    {
                        var timestamps = alt.timestamps;
                        AltStart = timestamps[0][1];
                        Transcript = "";
                        CurrentSpeaker = "0";
                        foreach (var ts in timestamps) //1つのalternatives中のtimesamps列挙
                        {
                            //話者表示（上で作ったディクショナリで話者が切り替わってないかチェック）
                            if (speaker[ts[1]] == CurrentSpeaker)
                            {
                                //切り替わってなければtimestamps単位の発話を追加
                                Transcript += ts[0];
                            }
                            else
                            {
                                //切り替わってたら行替え
                                PrintText(AltStart, CurrentSpeaker, Transcript);
                                Transcript = ts[0];
                                CurrentSpeaker = speaker[ts[1]]; //最後の話者
                            }
                        }
                    }
                    if (Transcript != "") {
                        PrintText(AltStart, CurrentSpeaker, Transcript);
                    }
                }
                //テキストファイル書き出し
                System.IO.StreamWriter sw = new System.IO.StreamWriter(
                    filename + ".txt",
                    false,
                    System.Text.Encoding.GetEncoding("UTF-8"));
                sw.Write(SaveText);
                //閉じる
                sw.Close();
            }
        }

    }


}

