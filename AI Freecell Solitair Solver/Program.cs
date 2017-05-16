using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    static class Program
    {
        private static String sMethod; // μεταβλητή που θα περιέχει τον προτιμόμενο αλγόριθμο για χρήση.
        private static String inputFileName; // αρχείο από το οποίο θα γίνει η ανάγνωση της κατάστασης του παιχνιδιού.
        private static String outputFileName; // αρχείο στο οποίο θα γραφτούν τα βήματα μέχρι την επίλυση του προβλήματος.

        private static State initialState = null; // θα περιλαμβάνει την αρχική κατάσταση του παιχνιδιού.

        private static Αuxiliary.methodEnum method = Αuxiliary.methodEnum.Empty;

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintHelp();
                Environment.Exit(0);
            }
            
            System.Console.WriteLine("Ok..begining....");

            // αποθήκευση των παραμέτρων.
             sMethod = args[0];
             inputFileName = args[1];
             outputFileName = args[2];

            
        /*    sMethod = "astar";
            inputFileName = "test10.txt";
            outputFileName = "test10MYsolastar.txt";
*/
            // κατασκευάζω το state.
            initialState = new State(inputFileName);

            solveProblem(); // αναζήτηση λύσης.
        }

        /// <summary>
        /// Εκτύπωση μιας ενδεικτικής βοήθειας για την σωστή κλήση της εφαρμογής.
        /// </summary>
        private static void PrintHelp()
        {
            System.Console.WriteLine("Error..........\n");
            System.Console.WriteLine("Wrong number of arguments. Use correct syntax:");
            System.Console.WriteLine("usage:\n\n\t AI Freecell Solitair Solver.exe <method> <inputFileName> <outputFileName>\n\n");

            System.Console.WriteLine("where: \n\n\t <method>  --> select one of { breadth, depth, best, astar }");
            System.Console.WriteLine("\t <inputFileName> --> select the name of the input file for reading the game state.");
            System.Console.WriteLine("\t <outputFileName> --> select a name (whatever) for the output file (it will contain the solution, if there is one.)");
            System.Console.WriteLine("e.g. the call \n\n\t AI Freecell Solitair Solver.exe breadth test1.txt test1sol.txt");
            System.Console.WriteLine("Will load the file test1.txt and will try to find a solution using breadth-first algorithm. The solution will be in file test1sol.txt.");
        }

        /// <summary>
        /// Η μέθοδος αυτή εξετάζει τους κόμβους στο μέτωπο αναζήτησης μέχρι να βρει λύση στο παιχνίδι ή να φτάσει σε timeout.
        /// </summary>
        private static void solveProblem()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start(); // ξεκινάει η χρονομέτρηση.

            Frontier gameFrontier = new Frontier();
            Αuxiliary.gameHistory = new Frontier();

            int step = 0;

            gameFrontier.AddLast(initialState);
            method = getMethod();

            // Επανάληψη που εξετάζει όλους τους κόμβους (καταστάσεις) που υπάρχουν στο μέτωπο αναζήτησης.
            while (gameFrontier.Count > 0)
            {
                State currentState = gameFrontier.Pop();
                Αuxiliary.gameHistory.AddLast(currentState);
                step++;

                if (step % 100 == 0)
                    System.Console.WriteLine("Step " + step);

                // αν έχει βρεθεί λύση.
                if (currentState.Status() == State.stateStatusEnum.SolutionFound)
                {
                    System.Console.WriteLine("Βρέθηκε λύση σε " + Convert.ToString(stopWatch.ElapsedMilliseconds / 1000) + " δευτερόλεπτα");
                    State lastState = currentState;
                    State t = lastState.Parent;
                    int totalSteps = 0;
                    while (t.Parent != initialState)
                    {
                        t.SolutionChild = lastState;
                        lastState = t;
                        t = lastState.Parent;
                        totalSteps++;

                    }
                    System.Console.WriteLine("Βήματα από την αρχική κατάσταση εως τη λύση " + Convert.ToString(totalSteps));
                    System.Console.WriteLine("Κόμβοι που εξετάστηκαν: " + Convert.ToString(step));
                    t.SolutionChild = lastState;
                    string solutionText = totalSteps.ToString() + Environment.NewLine;

                    // μαζεύω τα βήματα μέχρι την λύση.
                    while (t.SolutionChild != null)
                    {
                        //Stopwatch s = new Stopwatch();
                        //s.Start();

                        solutionText = solutionText + t.TotalMove() + Environment.NewLine;
                        t = t.SolutionChild;

                    }

                    // εξαγωγή σε αρχείο.
                    solutionText = solutionText + t.TotalMove() + Environment.NewLine;
                    System.IO.File.WriteAllText(outputFileName, solutionText);
                    System.Diagnostics.Process.Start(outputFileName);
                    return;
                } // if

                // αν δεν βρέθηκε λύση εξετάζω τα παιδιά του κόμβου ανάλογα με τον αλγόριθμο πάντα.
                Frontier childrenFrontier = currentState.GetChildrenStates(method);
                switch (method)
                {
                    case Αuxiliary.methodEnum.Breadth: // αν έχει επιλεγεί πρώτα σε πλάτος, τότε προσθέτω στην αρχή (εξετάζεται στο τέλος) του μετώπου το State εκείνο.
                        foreach (State state in childrenFrontier)
                            gameFrontier.AddFirst(state);
                        break;

                    case Αuxiliary.methodEnum.Depth: // αν έχει επιλεγεί πρώτα σε βάθος, τότε βάζω το State στο τέλος (άρα θα είναι το επόμενο που θα εξεταστεί).
                        foreach (State state in childrenFrontier)
                            gameFrontier.AddLast(state);
                        break;

                    case Αuxiliary.methodEnum.AStar: // Για ευρετικούς αλγορίθμους προσθέτω το Stat στο τέλος και μετά ταξινομώ με βάση την ευρετική συνάρτηση.
                        foreach (State state in childrenFrontier)
                            gameFrontier.AddLast(state);
                        gameFrontier = gameFrontier.SortedFrontier(method);
                        break;

                    case Αuxiliary.methodEnum.Best: // Για ευρετικούς αλγορίθμους προσθέτω το State στο τέλος και μετά ταξινομώ με βάση την ευρετική συνάρτηση.
                        foreach (State state in childrenFrontier)
                            gameFrontier.AddLast(state);
                        gameFrontier = gameFrontier.SortedFrontier(method);
                        break;

                    case Αuxiliary.methodEnum.Empty:
                        break;
                }


                if (stopWatch.ElapsedMilliseconds > Αuxiliary.TIMEOUT * 1000)
                {
                    System.Console.WriteLine("Δεν ήταν δυνατή η επίλυση του προβλήματος σε " + Convert.ToString(Αuxiliary.TIMEOUT) + " δευτερόλεπτα");
                    return;
                }

            } // while
        } // solveProblem method

        /// <summary>
        /// Μέθοδος που επιστρέφει τον επιλεγμένο αλγόριθμο αναζήτησης.
        /// </summary>
        /// <returns> τον αλγόριθμο αναζήτησης. </returns>
        public static Αuxiliary.methodEnum getMethod()
        {
            Αuxiliary.methodEnum resultMethod = Αuxiliary.methodEnum.Empty;

            if (sMethod.Equals("breadth"))
                resultMethod = Αuxiliary.methodEnum.Breadth;
            else if (sMethod.Equals("depth"))
                resultMethod = Αuxiliary.methodEnum.Depth;
            else if (sMethod.Equals("best"))
                resultMethod = Αuxiliary.methodEnum.Best;
            else if (sMethod.Equals("astar"))
                resultMethod = Αuxiliary.methodEnum.AStar;

            return resultMethod;
        }

    } // Program class
} // namespace
