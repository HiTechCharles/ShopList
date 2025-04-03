using System;

using System.IO;  //for streamreader/writer
namespace ShopList
{
    class Program
    {
        #region Variable declarations and explanation
        /*
         * Business Shopping List by Charles Martin
         * HiTechCharles@gmail.com
         * 
         * This program creates a shopping list for a snack
         * business.  The available products, and the price
         * are stored in a CSV file.  The user is asked how
         * many of each item they want to buy.  At the end,
         * a text file is made with the desired products and
         * A total cost of all items.
         * 
         * Sample CSV file in onedrive\documents\shoplist\stores directory
         * ITEM, QTY, PRICE
         * Sam's Club
         * Candy bars,30,21.98
         * apple juice,24,13.99
         * chips,30,13.38 */

        public static string[] Name = new string[99];  //product names
        public static Double[] Qty = new Double[99];  //qty per case
        public static Double[] Price = new Double[99];  //item price
        public static Double[] QtySelected = new Double[99];  //qty selected to buy
        //cur list total, # items,  all lists total, all lists items, budget
        public static Double ListTotal, ItemTotal, AllItems, AllTotal, Budget;
        public static int NumItems;  //number of items in file
        public static String StoreName;  //name of current store 
        //directory where store files are 
        public static string StorePath = Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\brew for you\\shoplist\\";
        static String UnderOver;  //budget over or under 
        public static Double BudgetDiff;  //budget difference
        public static string BusinessName;  //name of business
        public static string ShopDate;  //date to go shopping 
        #endregion

        static void Main()  //entry point of program 
        {
            // Get all command-line arguments
            string[] cmdArgs = Environment.GetCommandLineArgs();

            // Check if the parameter '-p' exists
            if (Array.Exists(cmdArgs, arg => arg == "-p"))
            {
                StorePath = Environment.CurrentDirectory +  "\\shoplist\\";
            }

            Console.Title = "BFY Shopping List";  //change window title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console
            if (File.Exists(StorePath + "Shopping List.txt"))  //if there is a shopping list
                File.Delete(StorePath + "Shopping List.txt");  //already, delete it.

            Budget = 30;  //shopping budget, used to be a prompt for the value
            BusinessName = "SHOPPING LIST FOR ";

            DateTime today = DateTime.Today;
            DateTime closestMonday = GetClosestMonday(today);
            ShopDate = closestMonday.ToShortDateString();

            StreamWriter bn = File.AppendText(StorePath + "Shopping List.txt");  //write business name and
            bn.WriteLine("\n" + BusinessName + " " + ShopDate);  //date to top of file 
            bn.WriteLine();
            bn.Close();  //close file

            Console.WriteLine("This is the Brew for You Shopping List maker.");
            Console.WriteLine("This program creates a list with the items you");
            Console.WriteLine("need, along with number of items, and total cost.\n");
            Console.WriteLine("For each store, you will be asked if you need"); 
            Console.WriteLine("to shop there.  The entire list will be saved");
            Console.WriteLine("as a text file, and added to your weekly order.");

            Console.WriteLine("\nWhile shopping, use the following:");
            Console.WriteLine("0 to 99 - quantity to purchase");
            Console.WriteLine("   skip - all done at current store");
            Console.WriteLine(("   prev - go to previous item\n"));
            Console.WriteLine("Your shopping budget is $" + Budget.ToString() + "\n");

            foreach (string file in Directory.EnumerateFiles
               (@StorePath + "Stores", "*.csv"))  //loop through all csv files in stores directory
            {
                LoadFile(file);  //load csv file into arrays name,qty,price

                Console.WriteLine();  //ask user if items needed from loaded store
                Console.Write("Do you need items at " + StoreName + "?  (Y or N)  ");
                String input = Console.ReadLine();  //read a line from keyboard
                if (input.ToUpper().Equals("Y"))  //if first letter is Y
                {
                    GetValues();  //asks how many of each product to buy
                    GetTotals();  //figure out total list cost and # of items
                }
            }

            StreamWriter LW = File.AppendText(StorePath + "Shopping List.txt");  //append to file
            LW.WriteLine("\nThe entire list contains " + AllItems);
            LW.WriteLine("items for $" + AllTotal + ".  The total");
            //write total # of items and total cost

            //figure out if over/under budget and how much
            if (AllTotal > Budget)  //over budget
            {
                UnderOver = " Over ";
                BudgetDiff = AllTotal - Budget;
            }

            if (Budget > AllTotal) //under
            {
                UnderOver = " Under ";
                BudgetDiff = Budget - AllTotal;
            }

            if (Budget == AllTotal) //exact
            {
                UnderOver = " Exactly ";
                BudgetDiff = 0;
            }

            LW.WriteLine("is" + UnderOver + "the $" + Budget + " budget by $" + BudgetDiff.ToString("n2"));
            LW.Close();

            Console.WriteLine();  //print same info sent to file to screen
            Console.WriteLine("The entire list contains " + AllItems + " items for $" + AllTotal);
            Console.WriteLine("The list is" + UnderOver + "the $" + Budget + " budget by $" + BudgetDiff.ToString("n2"));
            Console.WriteLine("\nYour shopping list has been saved.  Now, run the");
            Console.WriteLine("Brew for You Order Creation Tool.  Press a key to continue...");
            Console.ReadKey();
        }

        static DateTime GetClosestMonday(DateTime date)
        {
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)date.DayOfWeek + 7) % 7;
            int daysSinceMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            return date.AddDays(daysUntilMonday);
        }

        static void GetTotals()  //get total cost of all desired items
        {
            StreamWriter LW = File.AppendText(StorePath + "Shopping List.txt");  //append to file
            {
                LW.WriteLine(StoreName);  //name of store
                for (int I = 0; I < NumItems; I++)  //for each item
                {
                    if (QtySelected[I] > 0)  //if qty selected >0
                    {
                        ListTotal += Price[I] * QtySelected[I];  //keep running total of
                        ItemTotal += QtySelected[I];            //# items and cost
                        LW.WriteLine("    " + QtySelected[I] + " " + Name[I] + " @ $" + Price[I].ToString("n2"));
                    }
                }
                LW.WriteLine("----------");
                LW.WriteLine(ItemTotal + " items for $" + ListTotal.ToString("n2"));
                LW.WriteLine("");
                LW.Close();
                AllItems += ItemTotal;  //totals for entire list
                AllTotal += ListTotal;  //after a list is done, print items and total
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine(StoreName + ":  " + ItemTotal + " items for $" + ListTotal.ToString("n2"));
                Console.WriteLine("Grand Total so far:  $" + AllTotal.ToString("n2"));
                Console.WriteLine("  Remaining Budget:  $" + (Budget - AllTotal).ToString("n2"));
            }
        }

        static void GetValues()  //gets how many of each product to buy
        {
            string input;  //take input; 

            Console.WriteLine("\nThe following items are sold at " + StoreName );
            for (int I = 0; I < NumItems; I++)  //for each item
                {
                    do
                    {
                        Console.Write(Name[I] + " - ");
                        input = Console.ReadLine();  //get a number

                        #region word matching
                    if (input == "skip" )  //do actions on certain words 
                    {
                        Console.WriteLine("Skipping the remaining items from " + StoreName);
                        return;
                    }
                    if ( (input == "prev") && (I > 1) )  //previous item, make sure you can't go past beginning of list 
                    {
                        Console.WriteLine("Going to previous item");
                        I--;
                    }
                    #endregion
                    } while (Double.TryParse(input, out QtySelected[I]) == false);

                if (QtySelected[I] < 0 || QtySelected[I] > 9)
                {
                    I--;
                }

            }
            }

        static void LoadFile(string FileName)  //read a csv file
            {
                //clear all storage arrays before each file loads
                Array.Clear(Name, 0, 99);
                Array.Clear(Qty, 0, 99);
                Array.Clear(Price, 0, 99);
                Array.Clear(QtySelected, 0, 99);
                ListTotal = 0;
                ItemTotal = 0;
                NumItems = 0; string line;  //stores current line read from file
                StreamReader CsvRip = new StreamReader
                (FileName);  //open a file

            StoreName = CsvRip.ReadLine();   
            while (!CsvRip.EndOfStream)  //loop until EOF
                {
                    line = CsvRip.ReadLine();  //read a line from file

                    string[] subs = line.Split(',');  //split string by ,
                //(I don't want no subs, a sub is a girl that cant get no  love from me )
                    Name[NumItems] = subs[0];  //store product name

                    //store quantity and price
                    Qty[NumItems] = Convert.ToDouble(subs[1]);
                    Price[NumItems] = Convert.ToDouble(subs[2]);
                    NumItems++;  //count number of lines
                }
            }

    } //end class
}     //end namespace