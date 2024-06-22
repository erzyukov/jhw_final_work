// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("M2Ei62HnqcCIaOUimbPC0u/fykxm+vA2yMYkOaS98ljq3tCxuoeheCM9Zpfopqx/AcRNeQjzpk1Zo8i4dLBzOi/j8FzR0B4J5bal5X9VIHqtwGHQUP+SXyZJULqtM8R5eLYC0wOxMhEDPjU6GbV7tcQ+MjIyNjMwsTI8MwOxMjkxsTIyM6vhs82y9tnWtc1GPnS531Kot5DvN9BioF2dMPwa0RippQivYjeGitYeB0ACrnG9awwSHq5dOtZciU47eDzpD9nso/I25unTxynvX3A/vXBLT04+rHMGVAsSuVnfN6zwo0mTNu+hgjLaIQeIIZRehybAt4/My0z4PPetfxCgMIttehOKHLiDOxUEtaRFnbYhxCasyMifRUjlODH1+jEwMjMy");
        private static int[] order = new int[] { 8,10,13,5,12,8,10,8,8,11,10,11,12,13,14 };
        private static int key = 51;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
