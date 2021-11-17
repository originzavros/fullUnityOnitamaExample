using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RngUtils 
{
    //takes in an integer range and gives back random numbers from that range without picking the same number twice
    //the start must be lower than the end for the range please
    public static List<int> GetNonRepeatingNumberFromRange(int start, int end, int numberOfResults)
    {
        List<int> range = new List<int>();
        List<int> final = new List<int>();
        while(start != end)
        {
            range.Add(start);
            start++;
        }
        int index;
        while(final.Count != numberOfResults && range.Count > 0)
        {
            index = Random.Range(0, range.Count-1);
            final.Add(range[index]);
            range.RemoveAt(index);
        }

        return final;
    }
}
