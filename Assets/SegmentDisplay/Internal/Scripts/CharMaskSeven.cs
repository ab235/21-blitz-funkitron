//    CharMaskSeven


namespace Leguar.SegmentDisplay {

	internal class CharMaskSeven {

		private const string characters="0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ _\"'`´’\u00B0|()[]-=/\\@.,";

		private static readonly string[] masks = new string[] {
			"1111110","0110000","1101101","1111001","0110011","1011011","1011111","1110000","1111111","1111011",
			"1110111","0011111","1001110","0111101","1001111","1000111","1111011","0110111","0000110","0111100","1010111","0001110","1010100","0010101","1111110","1100111","1110011","0000101","1011011","0001111","0111110","0011100","0101010","0110111","0111011","1101101",
			"0000000","0001000","0100010","0000010","0000010","0000010","0000010","1100011","0000110","1001110","1111000","1001110","1111000","0000001","0001001","0100101","0010011","1101111","0000100","0000100"
		};

		internal static string getCharMask(SegmentDisplay segmentDisplay, char chr) {

			chr=char.ToUpper(chr);

			if (chr=='6' && segmentDisplay.Mod_S7S14_SixNine) {
				return "0011111";
			}
			if (chr=='9' && segmentDisplay.Mod_S7S14_SixNine) {
				return "1110011";
			}
			if (chr=='7' && segmentDisplay.Mod_S7_Seven) {
				return "1110010";
			}

			int index=characters.IndexOf(chr);
			if (index>=0) {
				return masks[index];
			}

			return null;

		}

	}

}
