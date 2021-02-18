//// a "best of both worlds" kind of thing, with the lookup performance of a hashset and the iteration performance of a list.

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class HashList<T> : IEnumerable<T>
//{
//    // these two internal collections are synchronized
//    private List<T> InnerList;
//    private Dictionary<T, int> InnerDictionary; // key: T, as duplicated in list. Keys are super fast to look up in dictionaries. value: list index of that T
    

//	public void Add(T thingbeingadded)
//    {
//        if (InnerDictionary.ContainsKey(thingbeingadded)) { return; }

//        InnerList.Add(thingbeingadded);
//        InnerDictionary.Add(thingbeingadded, InnerList.Count - 1);
//    }

//    public void Remove(T thingbeingremoved)
//    {
//        if (!InnerDictionary.ContainsKey(thingbeingremoved)) { return; }

//    }
//}