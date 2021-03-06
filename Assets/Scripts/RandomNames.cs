class RandomNames {
	private static System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>()
			{
	"Arundell",
	  "Arv",
	  "Arva",
	  "Arvad",
	  "Arvell",
	  "Arvid",
	  "Arvie",
	  "Arvin",
	  "Arvind",
	  "Arvo",
	  "Arvonio",
	  "Arvy",
	  "Ary",
	  "Aryn",
	  "As",
	  "Asa",
	  "Asabi",
	  "Asante",
	  "Asaph",
	  "Asare",
	  "Aschim",
	  "Ase",
	  "Asel",
	  "Ash",
	  "Asha",
	  "Ashbaugh",
	  "Ashbey",
	  "Ashby",
	  "Ashelman",
	  "Ashely",
			};

	public static string GetRandomName() {
		return names[UnityEngine.Random.Range(0, names.Count - 1)];
	}
}