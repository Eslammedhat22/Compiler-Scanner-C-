using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanner
{
    public partial class Form1 : Form
    {
        DataTable T = new DataTable();
        public Form1()
        {
            InitializeComponent();
            T.Columns.Add("Token Value", typeof(string));
            T.Columns.Add("Token Type", typeof(string));
            dataGridView1.DataSource = T;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        OpenFileDialog ofd = new OpenFileDialog();
        private void Browse_Click(object sender, EventArgs e)
        {
           
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                filePath.Text = Global.path =ofd.FileName;

            }
        }
        
        private void Run_Click(object sender, EventArgs e)
        {
            
            T.Rows.Clear();
            dataGridView1.DataSource = T;
             if (!File.Exists(filePath.Text)|Path.GetExtension(filePath.Text)!=".txt") error.Visible = true;
            else
            {
                error.Visible=false ;
                Global.path =filePath.Text;
                scanner.scanFile();
                string outputPathFile = (Path.GetDirectoryName(Global.path) + "\\scanner_output.txt");
                System.IO.StreamWriter file = new System.IO.StreamWriter(@outputPathFile);
                while (!Global.done)
                {
                    scanner.getNextToken();
                    if (!Global.done)
                    {
                        T.Rows.Add(Global.tokenValue, Global.tokenType);
                        dataGridView1.DataSource = T;
                        file.WriteLine ("   "+Global.tokenValue+"    :    "+ Global.tokenType);
                    }
                }
                file.Close();

            }
        }


    }
    public class scanner
    {
        public static void getNextToken ()
        {

            int currentState = 0;
            /////////////////////////////////////////////////////////////////////////////////////
            while (Global.code.Length > Global.nextToken)
            {
                char c = Global.code[Global.nextToken];
                if (currentState == 0) // start state // this function does not handle any error at the beginning of the Token //
                {
                    if (char.IsDigit(c)) { currentState = 2; Global.tokenValue = c.ToString(); Global.tokenType = "Number"; }
                    else if (char.IsLetter(c)) { currentState = 3; Global.tokenValue = c.ToString(); Global.tokenType = "Identifier"; }
                    else if (c == ':') { currentState = 4; Global.tokenValue = c.ToString(); Global.tokenType = "Special Symbol"; }
                    else if (c == '+' | c == '-' | c == '*' | c == '/' | c == '<' | c == ';' | c == '=' | c == '(' | c == ')') {  Global.tokenValue = c.ToString(); Global.tokenType = "Special Symbol"; Global.nextToken++;return; }
                    else if (c == '{') { currentState = 1;}
                    
                }
                
                else if(currentState == 1) // Comment
                {
                    if(c=='}')  currentState = 0;
                }
                else if (currentState == 2) // integer number
                {
                    if (char.IsDigit(c)) Global.tokenValue += c.ToString();
                    else if (c == '.')
                    {
                        Global.tokenValue += c.ToString();
                        currentState = 5;
                    }
                    else return; 
                }
                else if (currentState == 3) // Identifier
                {
                    if (char.IsDigit(c) | char.IsLetter(c) | c == '_') Global.tokenValue += c.ToString();
                    else
                    {
                        if (Global.tokenValue == "if" | Global.tokenValue == "then" | Global.tokenValue == "else" | Global.tokenValue == "end" | Global.tokenValue == "repeat" | Global.tokenValue == "until" | Global.tokenValue == "read" | Global.tokenValue == "write") Global.tokenType = "Reserved Words"; // IMPLEMENT THE RESERVED WORDS
                        return;
                    }
                }
                else if (currentState == 4) // Assignment
                {
                    if (c == '=') { Global.tokenValue += c.ToString(); Global.nextToken++; return; }
                    else currentState = 0; // when he finds ':'
                }
                else if (currentState == 5) // Decimal point
                {
                    if (char.IsDigit(c)) { Global.tokenValue += c.ToString(); currentState = 6; }
                    else currentState = 0;
                    
                }
                else if (currentState == 6) // Decimal number
                {
                    if (char.IsDigit(c)) { Global.tokenValue += c.ToString(); }
                    else return;
                }
               
                Global.nextToken++;
            }

            if(Global.code.Length <= Global.nextToken)
            {
                Global.done = true;
            }
            else
            {
                Global.done = false;
            }

            
        }
        public static void scanFile()
        {
            Global.code = System.IO.File.ReadAllText(@Global.path)+" ";
            Global.nextToken = 0;
            Global.done = false;
        }
    }
    static class Global
    {
        public static int nextToken = 0;// pointer to the next token
        public static string code = ""; // line read to scan
        public static string path = ""; //path of the file
        public static string tokenValue=""; // contain the token value to be displayed
        public static string tokenType=""; // contain the token type to be displayed
        public static bool done = false;
    }
}
