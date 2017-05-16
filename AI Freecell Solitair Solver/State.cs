using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    public class State
    {
        public enum MoveEnum : byte { moveFoundation, moveCascade, moveEmtpyStack, moveFreecell, moveInvalid };
        string fileName;

        Stacks[] stacks; // Πίνακας που περιέχει τις στοίβες με τα φύλλα.
        Card[] freeCell; // Πίνακας που περιέχει τα φύλλα που βρίσκονται στα freecels.

        // μεταβλητές για τον ορισμό ευρετικής συνάρτησης.
        public float g; // βαθμός που κάθε κόμβος έχει ένα λιγότερο από τον πατέρα του.
        public float h; // τιμή ευρετικής συνάρτησης.
        public float f; // άθροισμα ευρετικής συνάρτησης + κόστος (βήματα προς εκείνο το σημείο).

        private State parent; // Γονέας του κόμβου (κατάσταση από την οποία προήλθε η τρέχουσα κατάσταση).

        public State Parent
        {
            get { return parent; }
            set
            {
                g = value.g + 1;
                parent = value;
            }
        }

        public State SolutionChild;

        public string Move;
        public MoveEnum MoveType;

        int ClubsFoundation, SpadesFoundation, HeartsFoundation, DiamondsFoundation;

        public enum stateStatusEnum : byte { SolutionNotFound, SolutionFound, StillSearching };

        /// <summary>
        /// Μέθοδος η οποία διαβάζει μια κατάσταση από ένα αρχείο.
        /// </summary>
        private void ReadFromFile()
        {
            string line;
            StreamReader file = new StreamReader(fileName);
            int i = 0;

            while ((line = file.ReadLine()) != null)
            {
                stacks[i] = new Stacks(line);
                i++;
            }

            file.Close();

            ClubsFoundation = -1;
            SpadesFoundation = -1;
            HeartsFoundation = -1;
            DiamondsFoundation = -1;

        }

        /// <summary>
        /// Κατασκευαστής χωρίς ορίσματα.
        /// </summary>
        public State()
        {
            stacks = new Stacks[Αuxiliary.stacksCount];
            freeCell = new Card[Αuxiliary.freeCellCount];
        }

        /// <summary>
        /// Κατασκευαστής της κλάσης με όρισμα.
        /// </summary>
        /// <param name="FileName"> Το όνομα του αρχείου που θα ψάξει για να διαβάσει.</param>
        public State(string FileName) : this()
        {
            fileName = FileName;
            ReadFromFile();
        }

        /// <summary>
        /// Μέθοδος που βρίσκει τα παιδιά της τρέχουσας κατάστασης και τα επιστρέφει.
        /// </summary>
        /// <param name="method"> όνομα μεθόδου για αναζήτηση. </param>
        /// <returns> τα παιδιά τύπου Frontier </returns>
        public Frontier GetChildrenStates(Αuxiliary.methodEnum method)
        {
            Frontier children = new Frontier();
            bool EmptyFreeCellExamined = false;

            #region loop freecells
            for (int j = 0; j < Αuxiliary.freeCellCount; j++)
            {
                Card freeCellCard = freeCell[j];
                if (freeCellCard != null)
                {

                    #region from freecell to foundation
                    switch (freeCellCard.Symbol)
                    {
                        case SymbolEnum.Clubs:
                            if (freeCellCard.FoundationRule(ClubsFoundation))
                            {
                                State childState = this.Clone();
                                childState.ClubsFoundation++;
                                childState.freeCell[j] = null;
                                childState.Parent = this;
                                childState.Move = freeCellCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Diamonds:
                            if (freeCellCard.FoundationRule(DiamondsFoundation))
                            {
                                State childState = this.Clone();
                                childState.DiamondsFoundation++;
                                childState.freeCell[j] = null;
                                childState.Parent = this;
                                childState.Move = freeCellCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Hearts:
                            if (freeCellCard.FoundationRule(HeartsFoundation))
                            {
                                State childState = this.Clone();
                                childState.HeartsFoundation++;
                                childState.freeCell[j] = null;
                                childState.Parent = this;
                                childState.Move = freeCellCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Spades:
                            if (freeCellCard.FoundationRule(SpadesFoundation))
                            {
                                State childState = this.Clone();
                                childState.SpadesFoundation++;
                                childState.freeCell[j] = null;
                                childState.Parent = this;
                                childState.Move = freeCellCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;
                    }
                    #endregion from freecell to foundation

                    #region from freecell to stack
                    for (int i = 0; i < Αuxiliary.stacksCount; i++)
                    {
                        if (this.stacks[i] != null && this.stacks[i].Count > 0)
                        {
                            Card sCard = this.stacks[i].Last();
                            if (((sCard != null) & (freeCellCard.StacksRule(sCard) == StacksRuleEnum.RuleBefore)) //stack αντίθετου χρώματος και μεγαλύτερης κατά 1 τιμής
                                || (sCard == null)) //Άδειο stack
                            {
                                {
                                    State childState = this.Clone();
                                    childState.stacks[i].AddLast(freeCellCard);
                                    childState.freeCell[j] = null;
                                    if (!childState.AlreadyInHistory())
                                    {
                                        childState.Parent = this;
                                        childState.Move = freeCellCard.ToString();
                                        childState.MoveType = MoveEnum.moveEmtpyStack;
                                        children.AddState(childState, method);
                                    }
                                    //freeCellCard.Show();
                                }
                            }
                        }
                    } // for
                    #endregion from freecell to stack
                }
                else
                {
                    #region from stack to freecell
                    if (!EmptyFreeCellExamined) //μόνο για ένα freecell να γίνει, όλα τα freecells είναι ισοδύναμα
                    {
                        for (int i = 0; i < Αuxiliary.stacksCount; i++)
                        {
                            if (this.stacks[i] != null && this.stacks[i].Count > 0)
                            {
                                if (this.stacks[i].Last != null)
                                {
                                    State childState = this.Clone();
                                    childState.freeCell[j] = childState.stacks[i].Pop();
                                    if (!childState.AlreadyInHistory())
                                    {
                                        childState.Parent = this;
                                        childState.Move = childState.freeCell[j].ToString();
                                        childState.MoveType = MoveEnum.moveFreecell;
                                        children.AddState(childState, method);
                                    }

                                }
                            }
                        } // for
                        EmptyFreeCellExamined = true;
                    }
                    #endregion from stack to freecell
                }
            }
            #endregion loop freecells

            #region loop stacks
            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                Card aCard;
                if ((this.stacks[i] == null) || (this.stacks[i].Count == 0))
                    aCard = null;
                else
                {
                    aCard = this.stacks[i].Last();

                    #region from stack to foundation
                    switch (aCard.Symbol)
                    {
                        case SymbolEnum.Clubs:
                            if (aCard.FoundationRule(ClubsFoundation))
                            {
                                State childState = this.Clone();
                                childState.ClubsFoundation++;
                                childState.stacks[i].RemoveLast();
                                childState.Parent = this;
                                childState.Move = aCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Diamonds:
                            if (aCard.FoundationRule(DiamondsFoundation))
                            {
                                State childState = this.Clone();
                                childState.DiamondsFoundation++;
                                childState.stacks[i].RemoveLast();
                                childState.Parent = this;
                                childState.Move = aCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Hearts:
                            if (aCard.FoundationRule(HeartsFoundation))
                            {
                                State childState = this.Clone();
                                childState.HeartsFoundation++;
                                childState.stacks[i].RemoveLast();
                                childState.Parent = this;
                                childState.Move = aCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;

                        case SymbolEnum.Spades:
                            if (aCard.FoundationRule(SpadesFoundation))
                            {
                                State childState = this.Clone();
                                childState.SpadesFoundation++;
                                childState.stacks[i].RemoveLast();
                                childState.Parent = this;
                                childState.Move = aCard.ToString();
                                childState.MoveType = MoveEnum.moveFoundation;
                                children.AddState(childState, method);
                            }
                            break;
                    }
                    #endregion from stack to foundation

                }

                #region from stack to another stack
                for (int j = Convert.ToByte(i + 1); j < Αuxiliary.stacksCount; j++)
                {
                    Card bCard;
                    if ((this.stacks[j] == null) || (this.stacks[j].Count == 0))
                        bCard = null;
                    else
                    {
                        bCard = this.stacks[j].Last();
                    }

                    int SourceStackId = 100;
                    int TargetStackId = 100;

                    if ((aCard == null) & (bCard == null)) //Αν και τα δύο stacks είναι κενά
                        continue;
                    else if ((aCard == null) && (bCard != null)) //Αν το 1ο είναι κενό ενώ το 2ο όχι
                    {
                        if (this.stacks[j].Count == 1)
                            continue;
                        else
                        {
                            SourceStackId = j;
                            TargetStackId = i;
                        }
                    }
                    //θέλουμε να βρούμε και την περίπτωση όπου ένα stack έχει μόνο μια κάρτα και ένα άλλο είναι κενό
                    //Σε αυτή την περίπτωση δε θέλουμε μετακίνηση
                    else if ((aCard != null) && (bCard == null)) //Αν το 2ο είναι κενό ενώ το 1ο όχι
                    {
                        if (this.stacks[i].Count == 1)
                            continue;
                        else
                        {
                            SourceStackId = i;
                            TargetStackId = j;
                        }
                    }
                    else //Αν κανένα δεν είναι κενό
                    {
                        switch (aCard.StacksRule(bCard))
                        {
                            case StacksRuleEnum.RuleBefore:
                                SourceStackId = j;
                                TargetStackId = i;
                                break;
                            case StacksRuleEnum.RuleAfter:
                                SourceStackId = i;
                                TargetStackId = j;
                                break;
                            default:
                                continue;
                                break;
                        }
                    }

                    State childState = this.Clone();
                    Card PreviousTargetCard = null;

                    if (childState.stacks[TargetStackId] == null)
                        childState.stacks[TargetStackId] = new Stacks();

                    if (childState.stacks[TargetStackId].Count > 0)
                        PreviousTargetCard = childState.stacks[TargetStackId].Last();

                    childState.stacks[TargetStackId].AddLast(this.stacks[SourceStackId].Last());
                    childState.stacks[SourceStackId].RemoveLast();

                    if (!childState.AlreadyInHistory())
                    {
                        childState.Parent = this;
                        if (childState.stacks[TargetStackId].Count == 1) //new Stack
                        {
                            childState.Move = childState.stacks[TargetStackId].Last().ToString();
                            childState.MoveType = MoveEnum.moveEmtpyStack;
                        }
                        else
                        {
                            childState.Move = childState.stacks[TargetStackId].Last().ToString() + " " + PreviousTargetCard.ToString();
                            childState.MoveType = MoveEnum.moveCascade;
                        }

                        children.AddState(childState, method);
                    }
                }
                #endregion from stack to another stack
            }
            #endregion loop stacks

            return children;
        }

        /// <summary>
        /// Μέθοδος που φτιάχνει ένα αντίγραφο μιας κατάστασης.
        /// </summary>
        /// <returns> το αντίγραφο </returns>
        public State Clone()
        {
            State resultState = new State();

            for (int i = 0; i < Αuxiliary.freeCellCount; i++)
            {
                if (this.freeCell[i] != null)
                    resultState.freeCell[i] = this.freeCell[i].Clone();
            }

            resultState.HeartsFoundation = this.HeartsFoundation;
            resultState.DiamondsFoundation = this.DiamondsFoundation;
            resultState.ClubsFoundation = this.ClubsFoundation;
            resultState.SpadesFoundation = this.SpadesFoundation;

            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                if (this.stacks[i] != null)
                    resultState.stacks[i] = this.stacks[i].Clone();
            }

            return resultState;
        }

        /// <summary>
        /// Μέθοδος που ελέγχει αν έχουμε βρει λύση στο πρόβλημα.
        /// </summary>
        /// <returns> το αποτέλεσμα μέχρι στιγμής. </returns>
        public stateStatusEnum Status()
        {
            stateStatusEnum resultStatus = stateStatusEnum.StillSearching;

            if (HeartsFoundation < 12 || ClubsFoundation < 12 || DiamondsFoundation < 12 || SpadesFoundation < 12)
                resultStatus = stateStatusEnum.SolutionNotFound;
            else
                resultStatus = stateStatusEnum.SolutionFound;

            return resultStatus;
        }

        /// <summary>
        /// Μέθοδος που ελέγχει αν μια κατάσταση υπάρχει ήδη.
        /// </summary>
        public bool AlreadyInHistory()
        {
            foreach (State stateInHistory in Αuxiliary.gameHistory.Reverse<State>())
            {
                if (this.IsEqual(stateInHistory))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Ελέγχει αν μια κατάσταση είναι ίδια με την τρέχουσα.
        /// </summary>
        /// <param name="bState"> Η κατάσταση προς σύγκριση. </param>
        public bool IsEqual(State bState)
        {
            if ((this.HeartsFoundation != bState.HeartsFoundation) ||
               (this.DiamondsFoundation != bState.DiamondsFoundation) ||
               (this.ClubsFoundation != bState.ClubsFoundation) ||
               (this.SpadesFoundation != bState.SpadesFoundation))
                return false;

            for (int i = 0; i < Αuxiliary.freeCellCount; i++)
            {
                if ((this.freeCell[i] == null) && (bState.freeCell[i] != null))
                    return false;

                if ((this.freeCell[i] == null) && (bState.freeCell[i] == null))
                    ; // do nothing
                else if (!this.freeCell[i].IsEqual(bState.freeCell[i]))
                    return false;

            }

            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                if ((this.stacks[i] == null || this.stacks[i].Count == 0) && (bState.stacks[i] != null && bState.stacks[i].Count > 0))
                    return false;
                else if ((this.stacks[i].Count == 0) && (bState.stacks[i] != null) && (bState.stacks[i].Count > 0) && (bState.stacks[i].Last() != null))
                    return false;
                else if ((bState.stacks[i] == null || bState.stacks[i].Count == 0) && (this.stacks[i] != null && this.stacks[i].Count > 0))
                    return false;
                else if (this.stacks[i].Count == 0 && bState.stacks[i].Count == 0)
                    ; // do nothing
                else if (!this.stacks[i].Last().IsEqual(bState.stacks[i].Last()))
                    return false;

            }

            return true;
        }

        /// <summary>
        /// Μέθοδος που επιστρέφει ένα string με την κίνηση που έγινε.
        /// Χρησιμοποιείται για γράψιμο σε αρχείο.
        /// </summary>
        /// <returns> την κίνηση που έγινε. </returns>
        public string TotalMove()
        {
            string resultString = "";

            switch (this.MoveType)
            {
                case MoveEnum.moveCascade:
                    resultString = String.Format("stack {0}", this.Move);
                    break;

                case MoveEnum.moveEmtpyStack:
                    resultString = String.Format("emptystack {0}", this.Move);
                    break;

                case MoveEnum.moveFreecell:
                    resultString = String.Format("freecell {0}", this.Move);
                    break;

                case MoveEnum.moveFoundation:
                    resultString = String.Format("foundation {0}", this.Move);
                    break;
            }

            return resultString;
        }

        /// <summary>
        /// Εδώ εξετάζεται εάν η μέθοδος είναι αλγόριθμος που χρησιμοποιεί ευρετική συνάρτηση.
        /// </summary>
        /// <param name="method"> αλγόριθμος </param>
        public void Calc(Αuxiliary.methodEnum method)
        {
            switch (method)
            {
                case Αuxiliary.methodEnum.Best:
                    h = CalcH();
                    break;

                case Αuxiliary.methodEnum.AStar:
                    h = CalcH();
                    f = g + h;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Μέθοδος που μετράει πόσα κενά freecell υπάρχουν.
        /// </summary>
        /// <returns> τον αριθμό των κενών freecell. </returns>
        public int CountEmptyFreeCells()
        {
            int result = 0;

            for (int i = 0; i < Αuxiliary.freeCellCount; i++)
            {
                if (this.freeCell[i] == null)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Μέθοδος η οποία μετράει τον αριθμό των ελεύθερων στοιβών.
        /// </summary>
        /// <returns> τον αριθμό των ελεύθερων στοιβών. </returns>
        public int CountEmptyStacks()
        {
            int result = 0;
            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                if (this.stacks[i] == null || this.stacks[i].Count == 0)
                    result = result++;
            }

            return result;
        }

        /// <summary>
        /// Μέθοδος που μετράει πόσα φύλλα έχουν μπει στα foundations.
        /// </summary>
        /// <returns> αριθμό φύλλων </returns>
        public int CountCardsAtFoundations()
        {
            return (HeartsFoundation + DiamondsFoundation + ClubsFoundation + SpadesFoundation + 4);
        }

        /// <summary>
        /// Υπολογίζει το άθροισμα των βαθμών των πρώτων καρτών κάθε στοίβας.
        /// </summary>
        public int SumOfTopCards()
        {
            int res = 0;
            int counter = 0;

            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                if (!(this.stacks[i] == null || this.stacks[i].Count == 0))
                {
                    if (counter < 4)
                    {
                        res = res + 13;
                        counter++;
                    }
                    else
                    {
                        res = res + 12;
                    }
                    res = res - this.stacks[i].Last().Rank;
                }
            }

            return res;
        }

        /// <summary>
        /// Μέθοδος που υπολογίζει πόσα φύλλα έχουν μπει στη σωστή θέση
        /// </summary>
        public float WellPlacedCards()
        {
            float res = 0;

            for (int i = 0; i < Αuxiliary.stacksCount; i++)
            {
                if (this.stacks[i] != null && this.stacks[i].Count > 0)
                {
                    Card aCard = null;
                    Card bCard = null;
                    foreach (Card card in this.stacks[i])
                    {
                        if (aCard == null)
                            aCard = card;
                        else
                        {
                            bCard = card;
                            if (bCard.StacksRule(aCard) == StacksRuleEnum.RuleAfter)
                            {
                                res++;
                            }
                            aCard = bCard;
                        }
                    } // foreach
                } // if
            } // for

            return res;
        }

        /// <summary>
        /// Ευρετική συνάρτηση. Δίνεται περισσότερη βαρύτητα στα πόσα φύλλα είναι
        /// καλά στημένα (επιτρεπτές κινήσεις π.χ. μαύρο 9 πάνω σε κόκκινο 10), στον
        /// αριθμό των κενών στοιβών, λίγο λιγότερο στα φύλλα που έχουν μαζευτεί στα 
        /// foundations, και ακόμα λιγότερο στο άθροισμα των βαθμών των καρτών που έχουμε 
        /// στις πρώτες κάρτες κάθε στοίβας.
        /// </summary>
        /// <returns> τιμή της ευρετικής συνάρτησης. </returns>
        public float CalcH()
        {
            float res = 0;

            res = res + 0.0f * CountEmptyFreeCells();
            res = res + 1.0f * CountEmptyStacks();
            res = res + 0.72f * CountCardsAtFoundations();


            res = res + 0.21f * SumOfTopCards();
            res = res + 1.05f * WellPlacedCards();

            if (this.MoveType == MoveEnum.moveFoundation)
                res = res + 1.0f;

            return res;
        }

    } // State class
} // namespace
