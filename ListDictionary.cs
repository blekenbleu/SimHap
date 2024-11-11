using Newtonsoft.Json;
using SimHub;
using System.Collections.Generic;
using Atlas;

namespace blekenbleu.Haptic
{
	public class ListDictionary : NotifyPropertyChanged
	{
		internal Dictionary<string, List<CarSpec>> inDict = new();

		internal string Jstring()	// ignore null values; indent JSON
		{
			return JsonConvert.SerializeObject(inDict, new JsonSerializerSettings
			{ Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore });
		}

		// initialize inDict
		internal string Init(BlekHapt h, Dictionary<string, List<CarSpec>> json)
		{
			inDict = json;
			int fi = inDict.ContainsKey(h.GameDBText) ? inDict[h.GameDBText].Count : 0;

			if (1 > fi)
				return $":  {inDict.Count} games, no {h.GameDBText} cars";

			// fill null specs from Atlas
			// set h.Save if redundant specs in JSON
			for (int i = 0; i < inDict[h.GameDBText].Count; i++)
			{
				bool unique = true, redundant = true;
				string why = "redundant";
				CarSpec cs, ds = inDict[h.GameDBText][i];

				if (i > inDict[h.GameDBText].FindIndex(x => x.id == ds.id))  // duplicate?
				{
					redundant = true;
					why = "duplicate";
				}
				else if (0 <= (fi = BlekHapt.Atlas.FindIndex(x => x.id == ds.id)))
				{
					cs = BlekHapt.Atlas[fi];

					if ((null == ds.name || "" == ds.name || "?" == ds.name) && null != cs.name)
						ds.name = cs.name;
					else {
						unique &= ds.name != cs.name;
						redundant &= ds.name == cs.name;
					}
					if ((null == ds.category || "" == ds.category || "?" == ds.category) && null != cs.category)
						ds.category = cs.category;
					else {
						unique &= ds.category != cs.category;
						redundant &= ds.category == cs.category;
					}
					if ((null == ds.order || "" == ds.order || "?" == ds.order) && null != cs.order)
						ds.order = cs.order;
					else {
						unique &= ds.order != cs.order;
						redundant &= ds.order == cs.order;
					}
					if ((null == ds.config || "" == ds.config || "?" == ds.config) && null != cs.config)
						ds.config = cs.config;
					else {
						unique &= ds.config != cs.config;
						redundant &= ds.config == cs.config;
					}
					if ((null == ds.drive || "" == ds.drive || "?" == ds.drive) && null != cs.drive)
						ds.drive = cs.drive;
					else {
						unique &= ds.drive != cs.drive;
						redundant &= ds.drive == cs.drive;
					}
					if ((null == ds.loc || "" == ds.loc || "?" == ds.loc) && null != cs.loc)
						ds.loc = cs.loc;
					else {
						unique &= ds.loc != cs.loc;
						redundant &= ds.loc == cs.loc;
					}
					if ((null == ds.redline || 0 == ds.redline) && null != cs.redline)
						ds.redline = cs.redline;
					else {
						unique &= ds.redline != cs.redline;
						redundant &= ds.redline == cs.redline;
					}
					if ((null == ds.maxrpm || 0 == ds.maxrpm) && null != cs.maxrpm)
						ds.maxrpm = cs.maxrpm;
					else {
						unique &= ds.maxrpm != cs.maxrpm;
						redundant &= ds.maxrpm == cs.maxrpm;
					}
					if ((null == ds.idlerpm || 0 == ds.idlerpm) && null != cs.idlerpm)
						ds.idlerpm = cs.idlerpm;
					else {
						unique &= ds.idlerpm != cs.idlerpm;
						redundant &= ds.idlerpm == cs.idlerpm;
					}
					if ((null == ds.hp || 0 == ds.hp) && null != cs.hp)
						ds.hp = cs.hp;
					else {
						unique &= ds.hp != cs.hp;
						redundant &= ds.hp == cs.hp;
					}
					if ((null == ds.ehp || 0 == ds.ehp) && null != cs.ehp)
						ds.ehp = cs.ehp;
					else {
						unique &= ds.ehp != cs.ehp;
						redundant &= ds.ehp == cs.ehp;
					}
					if ((null == ds.cc || 0 == ds.cc) && null != cs.cc)
						ds.cc = cs.cc;
					else {
						unique &= ds.cc != cs.cc;
						redundant &= ds.cc == cs.cc;
					}
					if ((null == ds.nm || 0 == ds.nm) && null != cs.nm)
						ds.nm = cs.nm;
					else {
						unique &= ds.nm != cs.nm;
						redundant &= ds.nm == cs.nm;
					}
					if ((null == ds.cyl || 0 == ds.cyl) && null != cs.cyl)
						ds.cyl = cs.cyl;
					else {
						unique &= ds.cyl != cs.cyl;
						redundant &= ds.cyl == cs.cyl;
					}
				}
				else redundant = false;
				h.Save |= (redundant || !unique);       // rewrite inDict in End()
				if (redundant)
				{
					inDict[h.GameDBText].RemoveAt(i--);
					Logging.Current.Info(BlekHapt.pname + ".ListDictionary.Init(" + h.myfile
						+ $"): {why} {h.GameDBText} CarID: " + ds.id);
				}
			}
			return $":  {inDict[h.GameDBText].Count} {h.GameDBText} cars";
		}
	}   // class ListDictionary
}
