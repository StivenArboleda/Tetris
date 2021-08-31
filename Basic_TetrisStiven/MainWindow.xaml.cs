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

        List<int> currentRow = null;
        List<int> currentColumn = null;

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

            KeyDown += Key_Down;
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


        //----------------------------------------------------
        //MOVER LA FIGURA EN LAS 4 DIRECCIONES DE LAS FLECHAS
        //----------------------------------------------------
        private void Key_Down(object sender, KeyEventArgs e)
        {

            if (!timer.IsEnabled) { return; }
            switch (e.Key.ToString())
            {
                case "Up":
                    rotation += 90;
                    if (rotation > 270) { rotation = 0; }
                    shapeRotation(rotation);
                    break;
                case "Down":
                    downPos++;
                    break;
                case "Right":
                    shapeLimit(); // REVISA SI CHOCA
                    if (!rightCollided) { leftPos++; }
                    rightCollided = false;
                    break;
                case "Left":
                    shapeLimit();// REVISA SI CHOCA
                    if (!leftCollided) { leftPos--; }
                    leftCollided = false;
                    break;
            }
            moveShape();
        }

        //----------------------------------------------------
        //ROTAR LAS FICHAS DE ACUERDO A SU POSIBILIDAD
        //----------------------------------------------------
        private void shapeRotation(int _rotation)
        {
            // Check if collided
            if (rotationCollided(rotation))
            {
                rotation -= 90;
                return;
            }

            if (arrayShapes[currentShapeNumber].IndexOf("I_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentTetromino = getVariableByString("I_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("T_") == 0)
            {
                currentTetromino = getVariableByString("T_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("S_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentTetromino = getVariableByString("S_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("Z_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentTetromino = getVariableByString("Z_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("J_") == 0)
            {
                currentTetromino = getVariableByString("J_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("L_") == 0)
            {
                currentTetromino = getVariableByString("L_Tetromino_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("O_") == 0) // Do not rotate this
            {
                return;
            }

            isRotated = true;
            //addShape(currentShapeNumber, leftPos, downPos);
        }
        private void moveShape()
        {
            leftCollided = false;
            rightCollided = false;

            shapeLimit();  // VERIFICA SI LLEGÓ AL LIMITE
            if (leftPos > (tetrisGridColumn - currentTetrominoWidth))
            {
                leftPos = (tetrisGridColumn - currentTetrominoWidth);
            }
            else if (leftPos < 0) { leftPos = 0; }

            if (bottomCollided)
            {
               // shapeStoped(); PARAR FIGURA
                return;
            }
            //ShapeAdd(currentShapeNumber, leftPos, downPos);
        }

        // Accede por el nombre.
        private int[,] getVariableByString(string variable)
        {
            return (int[,])this.GetType().GetField(variable).GetValue(this);
        }
        private void shapeLimit()
        {
            bottomCollided = checkLimit(0, 1);
            leftCollided = checkLimit(-1, 0);
            rightCollided = checkLimit(1, 0);
        }

        private bool checkLimit(int _leftRightOffset, int _bottomOffset)
        {
            Rectangle movingSquare;
            int squareRow = 0;
            int squareColumn = 0;
            for (int i = 0; i <= 3; i++)
            {
                squareRow = currentRow[i];
                squareColumn = currentColumn[i];
                try
                {
                    movingSquare = (Rectangle)Tetris.Children
                    .Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == squareRow + _bottomOffset && Grid.GetColumn(e) == squareColumn + _leftRightOffset);
                    if (movingSquare != null)
                    {
                        if (movingSquare.Name.IndexOf("arrived") == 0)
                        {
                            return true;
                        }
                    }
                }
                catch { }
            }
            if (downPos > (tetrisGridRow - currentTetrominoHeigth)) { return true; }
            return false;
        }

        private bool rotationCollided(int _rotation)
        {
            if (checkLimit(0, currentTetrominoWidth - 1)) { return true; }//LLEGÓ AL FINAL 
            else if (checkLimit(0, -(currentTetrominoWidth - 1))) { return true; }// TOCA ARRIBA = PERDER
            else if (checkLimit(0, -1)) { return true; }// TOCA ARRIBA = PERDER
            else if (checkLimit(-1, currentTetrominoWidth - 1)) { return true; }// TOCA IZQUIERDA
            else if (checkLimit(1, currentTetrominoWidth - 1)) { return true; }// TOCA DERECHA
            return false;
        }

        //
        private void Start_game(object sender, RoutedEventArgs e)
        {

            if (isGameOver)
            {
                Tetris.Children.Clear();
                NextShape.Children.Clear();
                GameOver.Visibility = Visibility.Collapsed;
                isGameOver = false;
            }
            if (!timer.IsEnabled)
            {
                if (!gameActive)
                {
                    Score_num.Text = "0"; leftPos = 3; //addShape(currentShapeNumber, leftPos); }
                    Next.Visibility = Level.Visibility = Visibility.Visible;
                    Level.Text = "Level: " + gameLevel.ToString();
                    timer.Start();
                    start.Content = "Stop Game";
                    gameActive = true;
                }
                else
                {
                    timer.Stop();
                    start.Content = "Start Game";
                }
            }
        }

    }
}
