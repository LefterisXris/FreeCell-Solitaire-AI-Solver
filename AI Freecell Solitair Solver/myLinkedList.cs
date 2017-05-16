using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    /// <summary>
    /// Συνδεδεμένη λίστα η οποία βοηθάει για την διαχείρηση της στοίβας των φύλλων.
    /// </summary>
    /// <typeparam name="T"> Object </typeparam>
    public class myLinkedList<T> : LinkedList<T>
    {
        /// <summary>
        /// Μέθοδος που διαγράφει το τελευτείο στοιχείο της λίστας. Όπως ακριβώς pop() για στοίβες.
        /// </summary>
        public T Pop()
        {
            T resultT = this.Last();
            this.RemoveLast();
            return resultT;
        }

    } // myLinkedList class
} // namespace
