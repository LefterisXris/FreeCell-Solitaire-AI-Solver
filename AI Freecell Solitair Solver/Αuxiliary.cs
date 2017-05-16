using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{

    /// <summary>
    /// Κοινή δομή που χρησιμοποιείται για να έχουν όλες οι κλάσεις πρόσβαση σε μεταβλητές που χρειάζομαι συνέχεια.
    /// </summary>
    public static class Αuxiliary
    {
        public enum methodEnum { Breadth, Depth, AStar, Best, Empty }; // διαθέσιμες μέθοδοι αναζήτησης.
        public static int stacksCount = 8; // αριθμός στοιβών για φύλλα
        public static int freeCellCount = 4; // αριθμός freecel
        public static int TIMEOUT = 600; // σε πόσο χρόνο να σταματήσει η αναζήτηση.
        public static Frontier gameHistory; // ποιους κόμβους έχω ελέγξει.

    } // Auxiliary class
} // namespace
