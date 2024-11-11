using System.Collections.Generic;
using System.IO;
using Atlas;

namespace blekenbleu.Haptic
{
	public partial class Spec
	{
		CarSpec tc;
		bool nonnull = false;

		void Scrub(CarSpec ac)	// eliminate redundant or empty CarSpec values
		{
			nonnull = false;

			tc.game = null;
			if (null != tc.name && "" != tc.name && "?" != tc.name && (null == ac || tc.name != ac.name))
				nonnull = true;
			else tc.name = null;
			if (null != tc.category && "" != tc.category && "?" != tc.category && (null == ac || tc.category != ac.category))
				nonnull = true;
			else tc.category = null;
			if (null != tc.order && "" != tc.order && "?" != tc.order && (null == ac || tc.order != ac.order))
				nonnull = true;
			else tc.order = null;
			if (null != tc.config && "" != tc.config && "?" != tc.config && (null == ac || tc.config != ac.config))
				nonnull = true;
			else tc.config = null;
			if (null != tc.drive && "" != tc.drive && "?" != tc.drive && (null == ac || tc.drive != ac.drive))
				nonnull = true;
			else tc.drive = null;
			if (null != tc.loc && "" != tc.loc && "?" != tc.loc && (null == ac || tc.loc != ac.loc))
				nonnull = true;
			else tc.loc = null;
			if (null != tc.notes && "" != tc.notes && "?" != tc.notes && (null == ac || tc.notes != ac.notes))
				nonnull = true;
			else tc.notes = null;
			if (null != tc.redline && 0 != tc.redline && (null == ac || tc.redline != ac.redline))
				nonnull = true;
			else tc.redline = null;
			if (null != tc.maxrpm && 0 != tc.maxrpm && (null == ac || tc.maxrpm != ac.maxrpm))
				nonnull = true;
			else tc.maxrpm = null;
			if (null != tc.idlerpm && 0 != tc.idlerpm && (null == ac || tc.idlerpm != ac.idlerpm))
				nonnull = true;
			else tc.idlerpm = null;
			if (null != tc.hp && 0 != tc.hp && (null == ac || tc.hp != ac.hp))
				nonnull = true;
			else tc.hp = null;
			if (null != tc.ehp && 0 != tc.ehp && (null == ac || tc.ehp != ac.ehp))
				nonnull = true;
			else tc.ehp = null;
			if (null != tc.cc && 0 != tc.cc && (null == ac || tc.cc != ac.cc))
				nonnull = true;
			else tc.cc = null;
			if (null != tc.nm && 0 != tc.nm && (null == ac || tc.nm != ac.nm))
				nonnull = true;
			else tc.nm = null;
			if (null != tc.cyl && 0 != tc.cyl && (null == ac || tc.cyl != ac.cyl))
				nonnull = true;
			else tc.cyl = null;
		}

		/* create a *new* custom partial `CarSpec` list for this game
		 ; - from cache and unique `CarIds` from game JSON
		 ;	- only non-null, non-zero CarSpec values differing from Atlas
		 ;	- scrub Cache and LD.inDict[GameDBText]
		 ;	- replace JSON for this game and store
		 */
		internal string End(string myfile, string GameDBText, BlekHapt h)
		{
			if (h.Changed)
				SaveCar();
			if (0 == Lcache.Count && !h.Save)
				return "(no changes)";

			List<CarSpec> tlc = new();
			bool noAt = 1 > BlekHapt.Atlas.Count;
			int ai, li = Lcache.Count;

			for (int i = 0; i < li; i++)	// accumulate Scrub()ed cache
			{
				tc = Lcache[i];
				if (noAt || 0 > (ai = BlekHapt.Atlas.FindIndex(x => x.id == tc.id)))
					Scrub(null);
				else Scrub(BlekHapt.Atlas[ai]);
				if (nonnull)
					tlc.Add(tc);
			}

			li = LD.inDict.ContainsKey(GameDBText) ? LD.inDict[GameDBText].Count : 0;
			for (int i = 0; i < li; i++)	// game JSON
            {
				tc = LD.inDict[GameDBText][i];
				if (1 > tlc.Count || 0 > tlc.FindIndex(x => x.id == tc.id))
				{
					// Scrub JSON CarSpecs not yet accumulated
					if (noAt || 0 > (ai = BlekHapt.Atlas.FindIndex(x => x.id == tc.id)))
                    	Scrub(null);
					else Scrub(BlekHapt.Atlas[ai]);
					if (nonnull)
                    	tlc.Add(tc);
				}
			}
			
			if (LD.inDict.ContainsKey(GameDBText))
				LD.inDict.Remove(GameDBText);
			if (0 < tlc.Count)
				LD.inDict.Add(GameDBText, tlc);

			string sjs = LD.Jstring();
			if (0 == sjs.Length || "{}" == sjs)
				return $"JSON Serializer failure:  {LD.inDict.Count} games, {tlc.Count} {GameDBText} cars";

			File.WriteAllText(myfile /*= "R:/Temp/slim.json"*/, sjs);
			return $"{LD.inDict.Count} games, including {tlc.Count} {GameDBText} car[s] to " + myfile;
		}	// End()
	}		// class
}			// namespace
