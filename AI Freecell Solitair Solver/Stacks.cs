using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    class Stacks : myLinkedList<Card> // extends
    {
        /// <summary>
        /// Κατασκευαστής στοίβας χωρίς όρισμα.
        /// </summary>
        public Stacks()
        {

        }

        /// <summary>
        /// Κατασκευαστής στοίβας με παράμετρο. Αποθηκεύει τα φύλλα στην στοίβα.
        /// </summary>
        /// <param name="line"> περιέχει όλα τα φύλλα μιας στοίβας, χωρισμένα με κενό, ως μια συμβολοσειρά. </param>
        public Stacks(string s)
        {
            string[] sCards = s.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            // προσθέτω όλες τις κάρτες στην δομή.
            foreach (string a in sCards)
            {
                Card newCard = new Card(a);
                this.AddLast(newCard);
            }
        }

        /// <summary>
        ///Μέθοδος η οποία δημιουργεί ένα αντίγραφο της τρέχουσας στοίβας.
        /// </summary>
        /// <returns> στοίβα </returns>
        public Stacks Clone()
        {
            Stacks resultStacks = new Stacks();

            foreach (Card c in this)
                resultStacks.AddLast(c.Clone());

            return resultStacks;
        }

    } // Stacks class
} // namespace
