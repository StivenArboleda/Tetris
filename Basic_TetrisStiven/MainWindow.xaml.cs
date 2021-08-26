using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Basic_TetrisStiven
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int GAMESPEED = 600;// milisegundos de la velocidad de la figura
        private int levelScale = 60;// cada 60 segundos el nivel inccrementa

        DispatcherTimer timer;
        Random shapeRandom;
        private int rowCount = 0;
        private int columnCount = 0;
        private int leftPos = 0;
        private int downPos = 0;
        private int currentTetrominoWidth;
        private int currentTetrominoHeigth;
        private int currentShapeNumber;
        private int nextShapeNumber;
        private int tetrisGridColumn;
        private int tetrisGridRow;
        private int rotation = 0;
        private bool gameActive = false;
        private bool nextShapeDrawed = false;
        private int[,] currentTetromino = null;
        private bool isRotated = false;
        private bool bottomCollided = false;
        private bool leftCollided = false;
        private bool rightCollided = false;
        private bool isGameOver = false;
        private int gameSpeed;
        private double gameSpeedCounter = 0;
        private int gameLevel = 1;
        private int gameScore = 0;

        //COLORES FIJOS DE CADA FIGURA
        private static Color O_ShapeColor = Colors.Green;
        private static Color I_ShapeColor = Colors.Red;
        private static Color T_ShepColor = Colors.Gold;
        private static Color S_ShepColor = Colors.Violet;
        private static Color Z_ShepColor = Colors.DeepSkyBlue;
        private static Color J_ShepColor = Colors.Cyan;
        private static Color L_ShepColor = Colors.LightSeaGreen;

        string[] arrayShapes = { "","O_Shape_0" , "I_Shape_0",
                                        "T_Shape_0","S_Shape_0",
                                        "Z_Shape_0","J_Shape_0",
                                        "L_Shape_0"
                                   };


        public MainWindow()
        {
            InitializeComponent();
            gameSpeed = GAMESPEED;
           
            // Inicialización del contador
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed); // 600 millisegundos
            timer.Tick += Timer_Tick; //TIEMPO DE CAÍDA
            tetrisGridColumn = Tetris.ColumnDefinitions.Count;
            tetrisGridRow = Tetris.RowDefinitions.Count;
            shapeRandom = new Random();
            currentShapeNumber = shapeRandom.Next(1, 8);
            nextShapeNumber = shapeRandom.Next(1, 8);
            Next.Visibility = Level.Visibility = GameOver.Visibility = Visibility.Collapsed;
        }

        //TIEMPO DE CAÍDA DE FIGURAS
        private void Timer_Tick(object sender, EventArgs e)
        {
            downPos++;
            //MOVER LAS FIGURAS
            if (gameSpeedCounter >= levelScale)
            {
                if (gameSpeed >= 50)
                {
                    gameSpeed -= 50;
                    gameLevel++;
                    Level.Text = "Nivel: " + gameLevel.ToString();
                }
                else { gameSpeed = 50; }
                timer.Stop();
                timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed);
                timer.Start();
                gameSpeedCounter = 0;
            }
            gameSpeedCounter += (gameSpeed / 1000f);

        }



        //DEFINICION DE CADA FIGURA Y SUS DIFERENTES ROTACIONES

        //---- L Shape------------                                      // FORMA FÍSICA

        public int[,] L_Shape_0 = new int[2, 3] {{0,0,1},               //     * 
                                                        {1,1,1}};       // * * *

        public int[,] L_Shape_90 = new int[3, 2] {{1,0},                // *  
                                                        {1,0},          // *
                                                        {1,1}};         // * *

        public int[,] L_Shape_180 = new int[2, 3] {{1,1,1},             // * * * 
                                                        {1,0,0}};       // *

        public int[,] L_Shape_270 = new int[3, 2] {{1,1},               // * * 
                                                       {0,1},           //   *
                                                       {0,1 }};         //   *

        //---- J shape------------                                      // FORMA FÍSICA

        public int[,] J_Shape_0 = new int[2, 3] {{1,0,0},               // * 
                                                     {1,1,1}};          // * * *

        public int[,] J_Shape_90 = new int[3, 2] {{1,1},                // * * 
                                                      {1,0},            // *
                                                      {1,0}};           // * 

        public int[,] J_Shape_180 = new int[2, 3] {{1,1,1},             // * * * 
                                                       {0,0,1}};        //     *

        public int[,] J_Shape_270 = new int[3, 2] {{0,1},               //   * 
                                                       {0,1},           //   *
                                                       {1,1 }};         // * *

        //---- T shape------------                                      // FORMA FÍSICA

        public int[,] T_Shape_0 = new int[2, 3] {{0,1,0},               //    * 
                                                     {1,1,1}};          //  * * *

        public int[,] T_Shape_90 = new int[3, 2] {{1,0},                //  * 
                                                      {1,1},            //  * *
                                                      {1,0}};           //  *  

        public int[,] T_Shape_180 = new int[2, 3] {{1,1,1},             // * * *
                                                       {0,1,0}};        //   * 

        public int[,] T_Shape_270 = new int[3, 2] {{0,1},               //   * 
                                                       {1,1},           // * *
                                                       {0,1}};          //   *

        //---- Z shape------------                                      // FORMA FÍSICA

        public int[,] Z_Shape_0 = new int[2, 3] {{1,1,0},               // * *
                                                     {0,1,1}};          //   * *

        public int[,] Z_Shape_90 = new int[3, 2] {{0,1},                //   *
                                                      {1,1},            // * *
                                                      {1,0}};           // *

        //---- S Shape------------                                      // FORMA FÍSICA

        public int[,] S_Shape_0 = new int[2, 3] {{0,1,1},               //   * *
                                                     {1,1,0}};          // * *

        public int[,] S_Shape_90 = new int[3, 2] {{1,0},                // *
                                                      {1,1},            // * *
                                                      {0,1}};           //   *

        //---- I Shape------------                                      // FORMA FÍSICA

        public int[,] I_Shape_0 = new int[2, 4] { { 1, 1, 1, 1 },       // * * * *
            { 0, 0, 0, 0 } };                                           

        public int[,] I_Shape_90 = new int[4, 2] {{ 1,0 },              // *  
                                                       { 1,0 },         // *
                                                       { 1,0 },         // *
                                                       { 1,0 }};        // *

        //---- O Shape------------                                      // FORMA FÍSICA

        public int[,] O_Shape = new int[2, 2] { { 1, 1 },               // * *
                                                    { 1, 1 }};          // * *

    }
}
