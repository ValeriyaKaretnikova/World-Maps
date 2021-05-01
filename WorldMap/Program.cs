using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;



/* 
  **************************************************************************************************************************************************
  The original task was taken from the coursera.org online free course in Java Programming "Computer Science: Programming with a Purpose" 
  by Princeton University: Week 4 (Input and Output: assignment 'world maps'). 
  All the input files provided by Princeton University.
  **************************************************************************************************************************************************
* Purpose: Program that reads boundary information of a country (or other geographic entity) from file input and plots the results to drawing. A country consists of a set of regions (e.g., states, provinces, or other administrative divisions), each of which is described by a polygon.
* Input: The first line contains two integers: width and height. The remaining part of the input is divided into regions.
            - The first entry in each region is the name of the region.
            - The next entry is an integer specifying the number of vertices in the polygon describing the region.
            - Finally, the region contains the x- and y-coordinates of the vertices of the polygon.
* Output:  Draw the polygons
* Author: Karetnikova Valeriya
* Last Modified: November 25, 2020
*/

namespace WorldMap
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup
            SetConsole();

            //variables
            int menuOption;
            string fileName = "";
            int pause;
            float scaleFactor = 1F;

            //Display Header
            Console.WriteLine("\t**************************");
            Console.WriteLine("\n\tWelcome to the Map Drawing");
            Console.WriteLine("\n\t**************************");
            //Main Menu Loop
            do
            {
                DisplayMainMenu();
                menuOption = GetMenuChoice();

                switch (menuOption)
                {
                    case 1:
                        fileName = "world.csv";
                        pause = 1;
                        scaleFactor = 1;
                        break;
                    case 2:
                        fileName = "canada.csv";
                        pause = 40;
                        scaleFactor = 0.7F;
                        break;
                    case 3:
                        fileName = "usa.csv";
                        pause = 100;
                        scaleFactor = 1;
                        break;
                    case 4:
                        fileName = "china.csv";
                        pause = 80;
                        scaleFactor = 1.1F;
                        break;
                    case 5:
                        fileName = "india.csv";
                        pause = 40;
                        scaleFactor = 0.9F;
                        break;
                    case 6:
                        fileName = "united-kingdom.csv";
                        pause = 40;
                        scaleFactor = 0.7F;
                        break;
                    case 7:
                        fileName = "russia.csv";
                        pause = 80;
                        scaleFactor = 1;
                        break;
                    case 8:
                        fileName = "philippines.csv";
                        pause = 30;
                        scaleFactor = 0.6F;
                        break;
                    default:
                        pause = 0;
                        break;
                }//end of switch

                if (menuOption != 9)
                {
                    if (File.Exists(fileName))
                    {
                        StreamReader reader = null;
                        try
                        {
                            //Open StreamReader
                            reader = File.OpenText(fileName);

                            //Draw Map
                            DrawMap(reader, fileName, pause, scaleFactor);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                    else
                    {
                        Console.WriteLine("The file {0} does not exist", fileName);
                    }
                }//end if
            } while (menuOption != 9);

            Console.WriteLine(" Goodbye...");
            Console.ReadLine();
        }//eom

        static public void SetConsole()
        {
            Console.Title = "World Map";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();

        }//end of SetConsole

        static public void DisplayMainMenu()
        {
            Console.WriteLine("\n Please, choose which map would you like to draw: ");
            Console.WriteLine("\t1. World");
            Console.WriteLine("\t2. Canada");
            Console.WriteLine("\t3. USA");
            Console.WriteLine("\t4. China");
            Console.WriteLine("\t5. India");
            Console.WriteLine("\t6. Great Britain");
            Console.WriteLine("\t7. Russia");
            Console.WriteLine("\t8. Phillippines");
            Console.WriteLine("\t9. Exit");
        }//end of DisplayMainMenu

        static public int GetMenuChoice()
        {
            //constants
            const int MinMenuOption = 1;
            const int MaxMenuOption = 9;
            //variables
            int menuOption;
            bool isValid = false;
            //do-while loop to get the valid menu option
            do
            {
                menuOption = GetSafeInt(" Enter your option: ");
                if (menuOption >= MinMenuOption && menuOption <= MaxMenuOption)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine(" Invalid choice. Try again (enter {0}-{1}).", MinMenuOption, MaxMenuOption);
                }
            } while (!isValid);
            return menuOption;
        }//end of GetMenuChoice

        static public int GetSafeInt(string prompt)
        {
            //variables
            bool isValid = false;
            int safeInt;
            //do-while loop to get the valid integer
            do
            {
                try
                {
                    Console.Write(prompt);
                    safeInt = int.Parse(Console.ReadLine());
                    isValid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine(" Invalid Input...Please, try again.");
                    safeInt = 0;
                }
            } while (!isValid);
            return safeInt;
        }// end of GetSafeInt

        static public char GetSafeChar(string prompt)
        {
            //variables
            bool isValid = false;
            char menuOption;
            //do-while loop to validate char option
            do
            {
                try
                {
                    Console.Write(prompt);
                    menuOption = char.Parse(Console.ReadLine().ToLower());
                    if (menuOption == 'y' || menuOption == 'n')
                    {
                        isValid = true;
                    }
                    else
                    {
                        Console.WriteLine(" Invalid choice. Try again(choose y or n).");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(" Invalid Input... Please, try again.");
                    menuOption = 'z';
                }
            } while (!isValid);
            return menuOption;
        }//end of GetSafeChar

        static public void DrawMap(StreamReader reader, string fileName, int pause, float scaleFactor)
        {
            //variables
            int width,
                height;
            string input;

            // Read pictures dimensions
            string canvasSize = reader.ReadLine(); //very first line in the file
            string[] canvasWidthandHeight = canvasSize.Split(',');
            width = int.Parse(canvasWidthandHeight[0]);
            height = int.Parse(canvasWidthandHeight[1]);

            //Create window form
            Form form = CreateForm();
            form.Show();

            //Create graphics object
            Graphics graphics = form.CreateGraphics();
            Pen pen = new Pen(Color.Black);

            //shift X coordinates to center
            float centerX = (form.Width - width) / 2;

            //Process file
            while (reader.ReadLine() != null)
            {
                reader.ReadLine(); //Line with name of the region or country
                string numberOfPoints = reader.ReadLine(); //Line with the number of polygons in each region
                string[] points = numberOfPoints.Split(',');

                int n = int.Parse(points[0]);
                float[] xvertices = new float[n]; 
                float[] yvertices = new float[n];
                PointF[] polygonPoints = new PointF[n];

                for (int i = 0; i < n; i++)
                {
                    input = reader.ReadLine(); //line with coordinates
                    string[] coordinates = input.Split(',');
                    xvertices[i] = float.Parse(coordinates[0]) * scaleFactor + centerX; //adjusting x-coordinates
                    yvertices[i] = (height - float.Parse(coordinates[1])) * scaleFactor;//adjusting y-coordinates

                    polygonPoints[i] = new PointF(xvertices[i], yvertices[i]);
                }
                graphics.DrawPolygon(pen, polygonPoints); //draw polygons for each region
                System.Threading.Thread.Sleep(pause); //pause to show the drawing process
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            form.Close();

        }//end of DrawMap

        static public Form CreateForm()
        {
            //constants
            const int FormWidth = 1300;
            const int FormHeight = 800;

            Form form = new Form();
            form.BackColor = Color.White;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Width = FormWidth;
            form.Height = FormHeight;
            form.StartPosition = FormStartPosition.CenterScreen;

            return form;
        }//end of CreateForm

    }//eoc
}//eon
