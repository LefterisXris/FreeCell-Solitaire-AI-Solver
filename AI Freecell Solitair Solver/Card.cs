using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Freecell_Solitair_Solver
{
    public enum SymbolEnum { Hearts, Diamonds, Spades, Clubs, Empty }; // Μεταβλητή που παίρνει μια από τις τιμές αυτές, και αντιπροσωπεύει το σύμβολο μιας κάρτας.
    public enum ColorEnum { Red, Black, Empty }; // Αντίστοιχα για το χρώμα μιας κάρτας.
    public enum StacksRuleEnum { RuleBefore, RuleAfter, RuleIrrelevant }; // Αντίστοιχα για κανόνα (ορίζει αν μπορεί να γίνει).


    class Card
    {
        static Card EmptyCard = new Card { Symbol = SymbolEnum.Empty, Rank = 100 };
        SymbolEnum symbol;

        public SymbolEnum Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        int rank;

        public int Rank
        {
            get { return rank; }
            set
            {
                rank = value;
            }
        }

        /// <summary>
        /// Κατασκευαστής κενού φύλλου.
        /// </summary>
        public Card()
        {
            symbol = SymbolEnum.Empty;
            rank = 100;
        }

        /// <summary>
        /// Κατασκευαστής φύλλου.
        /// </summary>
        /// <param name="stringCard"> συμβολοσειρά που περιέχει σύμβολο και αριθμό για ένα φύλλο</param>
        public Card(string stringCard)
        {
            symbol = readSymbol(stringCard[0]);
            rank = Convert.ToInt16(stringCard.Substring(1));
        }

        /// <summary>
        /// Μέθοδος που αποθηκεύει το σύμβολο ενός χαρτιού.
        /// </summary>
        /// <param name="charSymbol"> Η συμβολοσειρά από την οποία θα πάρει το σύμβολο </param>
        /// <returns> σύμβολο χαρτιού </returns>
        private SymbolEnum readSymbol(char ch)
        {
            SymbolEnum resultSymbol = SymbolEnum.Empty;
            switch (ch)
            {
                case 'H':
                case 'h':
                    resultSymbol = SymbolEnum.Hearts;
                    break;
                case 'D':
                case 'd':
                    resultSymbol = SymbolEnum.Diamonds;
                    break;
                case 'C':
                case 'c':
                    resultSymbol = SymbolEnum.Clubs;
                    break;
                case 'S':
                case 's':
                    resultSymbol = SymbolEnum.Spades;
                    break;
            }
            return resultSymbol;
        }

        /// <summary>
        /// Μέθοδος που εξάγει σαν string μια κάρτα.
        /// </summary>
        public override string ToString()
        {
            string resultString = "";
            switch (Symbol)
            {
                case SymbolEnum.Clubs:
                    resultString = "C";
                    break;
                case SymbolEnum.Diamonds:
                    resultString = "D";
                    break;
                case SymbolEnum.Hearts:
                    resultString = "H";
                    break;
                case SymbolEnum.Spades:
                    resultString = "S";
                    break;
            }
            resultString = resultString + Convert.ToString(rank);
            return resultString;
        }

        /// <summary>
        /// Φτιάχνει ένα αντίγραφο ενός φύλλου.
        /// </summary>
        /// <returns> το φυλλο αυτό </returns>
        public Card Clone()
        {
            Card resultCard = new Card();
            resultCard.rank = this.rank;
            resultCard.symbol = this.symbol;
            return resultCard;
        }

        /// <summary>
        /// Ορίζεται το χρώμα ενός φύλλου.
        /// </summary>
        /// <returns> το χρώμα αυτού του φύλου </returns>
        public ColorEnum Color()
        {
            ColorEnum resultColor = ColorEnum.Empty;
            switch (symbol)
            {
                case SymbolEnum.Diamonds:
                case SymbolEnum.Hearts:
                    resultColor = ColorEnum.Red;
                    break;
                case SymbolEnum.Spades:
                case SymbolEnum.Clubs:
                    resultColor = ColorEnum.Black;
                    break;
            }
            return resultColor;
        }

        /// <summary>
        /// Μέθοδος που ελέγχει αν ένα φύλλο έχει διαφορετικό χρώμα από το τρέχων φύλλο.
        /// </summary>
        /// <param name="aCard"> Το φύλλο προς σύγκριση με το τρέχων φύλλο. </param>
        public bool OppositeColor(Card aCard)
        {
            if ((this.Color() == ColorEnum.Red) && (aCard.Color() == ColorEnum.Black))
                return true;
            if ((this.Color() == ColorEnum.Black) && (aCard.Color() == ColorEnum.Red))
                return true;

            return false;

        }

        /// <summary>
        /// Ελέγχει ποια κίνηση μπορεί να γίνει.
        /// </summary>
        /// <param name="sCard"></param>
        /// <returns> δυνατή κίνηση </returns>
        public StacksRuleEnum StacksRule(Card sCard)
        {
            StacksRuleEnum resultRule = StacksRuleEnum.RuleIrrelevant;
            if (OppositeColor(sCard))
            {
                switch (this.rank - sCard.rank)
                {
                    case 1:
                        resultRule = StacksRuleEnum.RuleBefore;  //Το sCard μπορεί να μετακινηθεί σε εμένα
                        break;
                    case -1:
                        resultRule = StacksRuleEnum.RuleAfter;  //εγώ μπορώ να μετακινηθώ στο sCard
                        break;
                }
            }
            return resultRule;
        }

        /// <summary>
        /// Ελέγχει αν μπορεί το τρέχων φύλλο να μπει στο foundation.
        /// </summary>
        /// <param name="FoundationValue"> Ο αριθμός του τελευταίου φύλλου στο foundation. </param>
        public bool FoundationRule(int FoundationValue)
        {
            return this.rank == FoundationValue + 1;
        }

        /// <summary>
        /// Ελέγχει αν ένα φύλλο είναι το ίδιο με το τρέχων φύλλο.
        /// </summary>
        /// <param name="bCard"> Φύλλο προς σύγκριση με το τρέχων.</param>
        public bool IsEqual(Card bCard)
        {
            if (bCard == null)
                return false;
            else
                if ((this.symbol == bCard.symbol) && (this.rank == bCard.rank))
                return true;

            return false;

        }

    } // Card class
} // namespace
