namespace HonorBound {
	public static class HonorBoundAPI {
		public static HonorBoundConfigData GetModSettings() {
			return HonorBoundMod.Instance.ConfigJson.Data;
		}
	}
}
