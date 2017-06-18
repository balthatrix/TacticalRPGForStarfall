using UnityEngine;

using System.Collections;
using System.Collections.Generic;



namespace Util {
	public class UtilString  {
		public static string WordWrap(string input, int maxCharacters)
		{
			List<string> lines = new List<string>();

			if (!input.Contains(" ") && !input.Contains("\n"))
			{
				int start = 0;
				while (start < input.Length)
				{
					lines.Add(input.Substring(start, Mathf.Min(maxCharacters, input.Length - start)));
					start += maxCharacters;
				}
			}
			else
			{
				string[] paragraphs = input.Split('\n');

				foreach (string paragraph in paragraphs)
				{
					string[] words = paragraph.Split(' ');

					string line = "";
					foreach (string word in words)
					{
						if ((line + word).Length > maxCharacters)
						{
							lines.Add(line.Trim());
							line = "";
						}

						line += string.Format("{0} ", word);
					}

					if (line.Length > 0)
					{
						lines.Add(line.Trim());
					}
				}
			}
			return string.Join ("\n", lines.ToArray ());
		}


		public static string Capitalize(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}

		public static string StrSigned(int value) {
			return value.ToString ("+#;-#;0");
		}


		/// <summary>
		/// Turns an enum val to a more human readable string
		/// 
		/// </summary>
		/// <returns>The to readable.</returns>
		/// <param name="enumVal">Enum value.</param>
		/// <param name="dropFirst">Drop first amount of '_' seperated strings.</param>
		/// <typeparam name="T">T should be an enum type.</typeparam>
		public static string EnumToReadable<T>(T enumVal, int dropFirst=0, bool titlize=true) {
			string[] arr = enumVal.ToString ().Split ('_');
			string ret = "";
			for (int i = 0; i < arr.Length; i++) {
				if (i >= dropFirst) {
					if (titlize) {
						ret += Capitalize (arr [i].ToLower ()) + " ";
					} else {
						ret += arr [i].ToLower () + " ";
					}
				}
			}

			return ret.Substring(0,ret.Length-1);
		}
	}



}