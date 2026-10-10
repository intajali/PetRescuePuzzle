// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("kZ6ECYBG7QnM/vHOX3gXa3C0o7njORzTYOvpzyiB6TRRUzhBlYFw/oeCgB80/xAdl9+BMdF7vS+RgXzdOUOj9WXdsmDa/mhbTVGhXW0q5th0yIIEDfjmBOBm2E6e3FjXMMKK/eJQ0/Di39Tb+FSaVCXf09PT19LRUNPd0uJQ09jQUNPT0gIELtB2X2e5ZxCRhLQeuWtnItTKDdOndRRSw5qFbz/tosGoJ8mCmJ3gQMsG+5o8oA8Qz9/evYgyV2aS8f/BDwCnAg9Os4EtMTkHDnO+bxfw6gC9eEDfsClGStIxZlCvKJjn71K9HBcaoae83egRfU/G/I5fXusLVkJd2vCHM1SmaX5NG2WKSV+c2ToJW5txYeZ1O1F5ZwfuUNuoTdDR09LT");
        private static int[] order = new int[] { 4,8,9,13,9,9,8,12,9,13,13,11,12,13,14 };
        private static int key = 210;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
