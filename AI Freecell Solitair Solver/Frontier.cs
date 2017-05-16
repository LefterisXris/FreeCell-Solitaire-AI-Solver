using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    public class Frontier : myLinkedList<State>
    {

        /// <summary>
        /// Μέθοδος που επιστρέφει το μέτωπο ταξινομημένο. Με χρήση της ευρετικής συνάρτησης,
        /// το μέτωπο αναζήτησης ταξινομείται ώστε να γίνει επίσκεψη πρώτα στον κατάλληλο κόμβο ( βάση αλγορίθμου).
        /// </summary>
        /// <param name="method"> αλγόριθμος </param>
        /// <returns> το μέτωπο </returns>
        public Frontier SortedFrontier(Αuxiliary.methodEnum method)
        {
            Frontier resultFrontier = new Frontier();
            switch (method)
            {
                case Αuxiliary.methodEnum.Breadth:
                    resultFrontier = this;
                    break;

                case Αuxiliary.methodEnum.Depth:
                    resultFrontier = this;
                    break;

                case Αuxiliary.methodEnum.Best:
                    var test = from state in this orderby state.h select state;
                    foreach (State s in test)
                        resultFrontier.AddLast(s);
                    break;

                case Αuxiliary.methodEnum.AStar:
                    test = from state in this orderby state.f select state;
                    foreach (State s in test)
                        resultFrontier.AddLast(s);
                    break;
            }
            return resultFrontier;
        }

        /// <summary>
        /// Προσθέτει μια κατάσταση στο μέτωπο αναζήτησης.
        /// </summary>
        /// <param name="s"> η κατάσταση που θα προστεθεί.</param>
        /// <param name="method"> αλγόριθμος </param>
        public void AddState(State s, Αuxiliary.methodEnum method)
        {
            s.Calc(method);
            this.AddLast(s);
        }

    } // Frontier class
} // namespace
