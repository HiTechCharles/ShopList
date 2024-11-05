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
            Console.Title = "ShopList by Charles Martin";  //change window title

            if (File.Exists(StorePath + "Shopping List.txt"))  //if there is a shopping list
                File.Delete(StorePath + "Shopping List.txt");  //already, delete it.

Console.WriteLine("Welcome to ShopList! by Charles Martin\n");

            string input;  //used for several text inputs
            do
            {
                Console.Write("How much do you want to spend?  ");
                input = Console.ReadLine();  //get a number for budget 
            } while (Double.TryParse(input, out Budget) == false);  //keep looping until numeric input 

            BusinessName = GetBizNames();  //determine who we're shopping for 

            Console.Write("\nOn which date are you shopping?  ");
            ShopDate = Console.ReadLine();  //get date to shop

            StreamWriter bn = File.AppendText(StorePath + "Shopping List.txt");  //write business name and
            bn.WriteLine(BusinessName + " " + ShopDate);  //date to top of file 
            bn.WriteLine();
            bn.Close();  //close file

            Console.WriteLine("\nFor each store, you will be asked if you need"); 
            Console.WriteLine("to get things from there.  Use Y or N.");
            Console.WriteLine("The entire list will be put into notepad for printing.");

            Console.WriteLine("\nWhile shopping, use the following:");
            Console.WriteLine("0 to 99 - quantity to purchase");
            Console.WriteLine("   skip - all done at current store");
            Console.WriteLine(("   prev - go to previous item\n"));

             foreach (string file in Directory.EnumerateFiles
               (@StorePath + "Stores", "*.csv"))  //loop through all csv files in stores directory
            {
                LoadFile(file);  //load csv file into arrays name,qty,price

                Console.WriteLine();  //ask user if items needed from loaded store
                Console.Write("Do you need items at " + StoreName + "?  (Y or N)  ");
                input = Console.ReadLine();  //read a line from keyboard
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
            Console.WriteLine();

            Console.WriteLine("Press a key to exit and see the list using notepad.");
            Console.ReadKey();  //wait for any keypress

            System.Diagnostics.Process.Start(@StorePath + "Shopping List.txt");
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

        public static string GetBizNames()  //pick a business to shop for 
        {
            string[] GBN = new string[9];  //holds list of names in a file
            string line, bin;  //for taking inputs 
            int count=0, sel = 0;  //number of lines, selected index 
            bool ValidInput = false;  

            Console.WriteLine("\nWhich business are you shopping for?");
            StreamReader sr = new StreamReader(StorePath + "\\BusinessNames.txt");  //open file read 
            while (!sr.EndOfStream)  //loop until end of file
            {
                line = sr.ReadLine();  //read a line from file 
                GBN[count] = line;  //store it 
                Console.WriteLine("     " + count + " - " + GBN[count]);  //print index and line 
                count++;  //count lines 
            }

            sr.Close(); //close file 
            count--;  //count is at end of loop have to take 1 away 

            while (!ValidInput) //==true 
            {
                Console.Write("Make a selection (0 to " + count + ")  ");
                bin = Console.ReadLine();  //get a number 
                bool ValidRange = int.TryParse(bin, out sel);

                if ((sel < 0) || (sel > count))  //value out of range 
                {
                    ValidInput = false;
                }
                else
                {
                    //ValidInput = true;
                    return GBN[sel];
                }
            }
            return null;
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