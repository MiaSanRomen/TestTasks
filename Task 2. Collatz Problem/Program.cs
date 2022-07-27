using System;
using System.Collections.Generic;

namespace TestTask2
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<uint, uint> lengthFromEachNumber = new Dictionary<uint, uint> {{1, 1}};  //This dictionary will storage start numbers and 
            uint maximumChainLength = 0;                                                        //length of their chains. I use dictionary because
            uint maximumChainStartNumber = 0;                                                   //search complexity in it is O(1)
            for (uint startNumber = 2; startNumber <= 1000000; startNumber++)
            {
                uint chainNumber = startNumber;
                uint currentChainLength = 0;
                while (true)                            //As all chains lead to 1, we can use while(true)
                {
                    if (lengthFromEachNumber.ContainsKey(chainNumber))                 //If dictionary keys contains generated in chain number,  
                    {                                                                  //there is no need to generate the chin one more time
                        currentChainLength += lengthFromEachNumber[chainNumber];       //So we just add to this chain's length one already founded before
                        lengthFromEachNumber.Add(startNumber, currentChainLength);     //Add the start number and the length of his chain 
                        break;
                    }

                    if (chainNumber % 2 == 1)                             //Here program generate next number if current was not founded in dictionary
                    {
                        chainNumber = 3 * chainNumber + 1;
                    }
                    else
                    {
                        chainNumber = chainNumber / 2;
                    }

                    currentChainLength++;
                }

                if (maximumChainLength < currentChainLength)                //If length of current chain is greater, save it instead of previous maximum
                {
                    maximumChainLength = currentChainLength;
                    maximumChainStartNumber = startNumber;
                }
            }
            Console.WriteLine($"\nMaximum Chain = {maximumChainLength}");              
            Console.WriteLine($"Start Number = {maximumChainStartNumber}");
        }
    }
}
