### Εργασία στην Τεχνητή Νοημοσύνη. 
##### Λύση παιχνιδιού Freecell Solitaire με δυνατότητα επιλογής 4 αλγορίθμων.          Μάιος 2017



Σημείωση 1:  Τα εκτελέσιμα αρχεία καθώς και μερικά τεστακια  βρίσκονται στον φάκελο
    
    AI Freecell Solitair Solver\bin\Debug



Οδηγίες χρήσης:

Για να τρέξει η εφαρμογή, μέσω γραμμής εντολών χρησιμοποιήστε εντολή του τύπου "AI Freecell Solitair Solver.exe" <method> <inputFileName> <outputFileName> όπου:
•	<method>, μπορείτε να βάλετε breadth, depth, best, astar που είναι για την επιλογή αλγορίθμου αναζήτησης, 
•	< inputFileName>, το όνομα του αρχείου που θα περιέχει την αρχική κατάσταση, και 
•	<outputFileName>, το αρχείου που θα περιέχει την λύση (αν βρεθεί).

Παράδειγμα εκτέλεσης: 
"AI Freecell Solitair Solver.exe" best test1.txt test1SolBest.txt

Η εφαρμογή σταματάει όταν βρεθεί λύση (αν βρεθεί) ή αν περάσουν 600 δευτερόλεπτα που έχουν οριστεί ως TIMEOUT στην κλάση Auxiliary.cs.