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
        private int currentShapeWidth;
        private int currentShapeHeigth;
        private int currentShapeNumber;
        private int nextShapeNumber;
        private int tetrisGridColumn;
        private int tetrisGridRow;
        private int rotation = 0;
        private bool gameActive = false;
        private bool nextShapeDrawed = false;
        private int[,] currentShape = null;
        private bool isRotated = false;
        private bool bottomCollided = false;
        private bool leftCollided = false;
        private bool rightCollided = false;
        private bool isGameOver = false;
        private bool modeColor = false;
        private int gameSpeed;
        private double gameSpeedCounter = 0;
        private int gameLevel = 1;
        private int gameScore = 0;

        List<int> currentRow = null;
        List<int> currentColumn = null;

        //COLORES FIJOS DE CADA FIGURA
        private static Color O_ShapeColor = Colors.Green;
        private static Color I_ShapeColor = Colors.Red;
        private static Color T_ShapeColor = Colors.White;
        private static Color S_ShapeColor = Colors.Violet;
        private static Color Z_ShapeColor = Colors.Blue;
        private static Color J_ShapeColor = Colors.Chocolate;
        private static Color L_ShapeColor = Colors.LightSeaGreen;

        string[] arrayShapes = { "","O_Shape" , "I_Shape_0",
                                        "T_Shape_0","S_Shape_0",
                                        "Z_Shape_0","J_Shape_0",
                                        "L_Shape_0"
                                   };

        Color[] shapeColor = {  O_ShapeColor,I_ShapeColor,
                                T_ShapeColor,S_ShapeColor,
                                Z_ShapeColor,J_ShapeColor,
                                L_ShapeColor
                             };

        #region Array of tetrominos shape

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

        public object Task { get; private set; }
        #endregion

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

        //CONTROLA EL TIEMPO PARA PODER MOVER LA FIGURA
        //DE ACUERDO AL NIVEL DEL JUEGO
        private void Timer_Tick(object sender, EventArgs e)
        {
            downPos++;
            moveShape();
            if (gameSpeedCounter >= levelScale)
            {
                if (gameSpeed >= 50)
                {
                    gameSpeed -= 50;
                    gameLevel++;
                    Level.Text = "Nivel: " + gameLevel.ToString();
                    timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed);
                }
                else { gameSpeed = 50; }
                timer.Stop();
                timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed);
                timer.Start();
                gameSpeedCounter = 0;
            }
            gameSpeedCounter += (gameSpeed / 1000f);

        }

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
        // MÉTODOS PARA ROTAR LAS FICHAS DE ACUERDO A SU POSIBILIDAD
        //----------------------------------------------------
        private void shapeRotation(int _rotation)
        {
            // Check if collided
            if (rotationLimit(rotation))
            {
                rotation -= 90;
                return;
            }

            if (arrayShapes[currentShapeNumber].IndexOf("I_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentShape = getVariableByString("I_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("T_") == 0)
            {
                currentShape = getVariableByString("T_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("S_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentShape = getVariableByString("S_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("Z_") == 0)
            {
                if (_rotation > 90) { _rotation = rotation = 0; }
                currentShape = getVariableByString("Z_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("J_") == 0)
            {
                currentShape = getVariableByString("J_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("L_") == 0)
            {
                currentShape = getVariableByString("L_Shape_" + _rotation);
            }
            else if (arrayShapes[currentShapeNumber].IndexOf("O_") == 0) 
            {
                return;
            }

            isRotated = true;
            shapeAdd(currentShapeNumber, leftPos, downPos);
        }
        private void moveShape()
        {
            leftCollided = false;
            rightCollided = false;

            shapeLimit();  // VERIFICA SI LLEGÓ AL LIMITE
            if (leftPos > (tetrisGridColumn - currentShapeWidth))
            {
                leftPos = (tetrisGridColumn - currentShapeWidth);
            }
            else if (leftPos < 0) { leftPos = 0; }

            if (bottomCollided)
            {
                shapeStoped(); //PARAR FIGURA
                return;
            }
            shapeAdd(currentShapeNumber, leftPos, downPos);
        }
        private void shapeLimit()
        {
            bottomCollided = checkLimit(0, 1);
            leftCollided = checkLimit(-1, 0);
            rightCollided = checkLimit(1, 0);
        }
        private bool rotationLimit(int _rotation)
        {
            if (checkLimit(0, currentShapeWidth - 1)) { return true; }//LLEGÓ AL FINAL 
            else if (checkLimit(0, -(currentShapeWidth - 1))) { return true; }// TOCA ARRIBA = PERDER
            else if (checkLimit(0, -1)) { return true; }// TOCA ARRIBA = PERDER
            else if (checkLimit(-1, currentShapeWidth - 1)) { return true; }// TOCA IZQUIERDA
            else if (checkLimit(1, currentShapeWidth - 1)) { return true; }// TOCA DERECHA
            return false;
        }

        // Accede por el nombre.
        private int[,] getVariableByString(string variable)
        {
            return (int[,])this.GetType().GetField(variable).GetValue(this);
        }
  
        //------------------------------------------------------
        // MÉTODOS PARA REVISAR LOS LÍMITES DE LAS FIGURAS
        //------------------------------------------------------
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
            if (downPos > (tetrisGridRow - currentShapeHeigth)) { return true; }
            return false;
        }
        private void checkComplete()
        {
            int gridRow = Tetris.RowDefinitions.Count;
            int gridColumn = Tetris.ColumnDefinitions.Count;
            int squareCount = 0;
            for (int row = gridRow; row >= 0; row--)
            {
                squareCount = 0;
                for (int column = gridColumn; column >= 0; column--)
                {
                    Rectangle square;
                    square = (Rectangle)Tetris.Children
                   .Cast<UIElement>()
                   .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
                    if (square != null)
                    {
                        if (square.Name.IndexOf("arrived") == 0)
                        {
                            squareCount++;
                        }
                    }
                }

                // If squareCount == gridColumn this means tha the line is completed and must to be delete
                if (squareCount == gridColumn)
                {
                    deleteLine(row);
                    Score.Text = getScore().ToString();
                    checkComplete();
                }
            }
        }

        //-----------------------------------------------------------
        // MÉTODOS PARA EL FUNCIONAMIENTOS DEL JUEGO
        //-----------------------------------------------------------
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
                    Score_num.Text = "0"; leftPos = 3; shapeAdd(currentShapeNumber, leftPos);
                }
                Next.Visibility = Level.Visibility = Visibility.Visible;
                Level.Text = "Nivel: " + gameLevel.ToString();
                timer.Start();
                start.Content = "Pausar Juego";
                gameActive = true;
            }
            else
            {
                timer.Stop();
                start.Content = "Empezar";
            }
        }

        //AGREGA LA FIGURA A LA GRID
        private void shapeAdd(int shapeNumber, int _left = 0, int _down = 0)
        {
            // Elimina la posición anterior de la figura
            removeShape();
            currentRow = new List<int>();
            currentColumn = new List<int>();
            Rectangle square = null;
            if (!isRotated)
            {
                currentShape = null;
                currentShape = getVariableByString(arrayShapes[shapeNumber].ToString());
            }
            int firstDim = currentShape.GetLength(0);
            int secondDim = currentShape.GetLength(1);
            currentShapeWidth = secondDim;
            currentShapeHeigth = firstDim;
            // CASO ESPECIAL DE LA FIGURA I
            if (currentShape == I_Shape_90)
            {
                currentShapeWidth = 1;
            }
            else if (currentShape == I_Shape_0) { currentShapeHeigth = 1; }
           
            for (int row = 0; row < firstDim; row++)
            {
                for (int column = 0; column < secondDim; column++)
                {
                    int bit = currentShape[row, column];
                    if (bit == 1)
                    {
                        square = getBasicSquare(shapeColor[shapeNumber - 1]);
                        Tetris.Children.Add(square);
                        square.Name = "moving_" + Grid.GetRow(square) + "_" + Grid.GetColumn(square);
                        if (_down >= Tetris.RowDefinitions.Count - currentShapeHeigth)
                        {
                            _down = Tetris.RowDefinitions.Count - currentShapeHeigth;
                        }
                        Grid.SetRow(square, rowCount + _down);
                        Grid.SetColumn(square, columnCount + _left);
                        currentRow.Add(rowCount + _down);
                        currentColumn.Add(columnCount + _left);

                    }
                    columnCount++;
                }
                columnCount = 0;
                rowCount++;
            }
            columnCount = 0;
            rowCount = 0;
            if (!nextShapeDrawed)
            {
                drawNextShape(nextShapeNumber);
            }
        }
        // DIBUJA LA SIGUIENTE FIGURA
        private void drawNextShape(int shapeNumber)
        {
            NextShape.Children.Clear();
            int[,] nextShape = null;
            nextShape = getVariableByString(arrayShapes[shapeNumber]);
            int firstDim = nextShape.GetLength(0);
            int secondDim = nextShape.GetLength(1);
            int x = 0;
            int y = 0;
            Rectangle square;
            for (int row = 0; row < firstDim; row++)
            {
                for (int column = 0; column < secondDim; column++)
                {
                    int bit = nextShape[row, column];
                    if (bit == 1)
                    {
                        square = getBasicSquare(shapeColor[shapeNumber - 1]);
                        NextShape.Children.Add(square);
                        Canvas.SetLeft(square, x);
                        Canvas.SetTop(square, y);
                    }
                    x += 36;
                }
                x = 0;
                y += 36;
            }
            nextShapeDrawed = true;
        }

        private void removeShape() 
        {
            int index = 0;
            while (index < Tetris.Children.Count)
            {
                UIElement element = Tetris.Children[index];
                if (element is Rectangle)
                {
                    Rectangle square = (Rectangle)element;
                    if (square.Name.IndexOf("moving_") == 0)
                    {
                        Tetris.Children.Remove(element);
                        index = -1;
                    }
                }
                index++;
            }
        }

        //Crea el cuadro para tetris
        private Rectangle getBasicSquare(Color rectColor)
        {
            Rectangle rectangle = new Rectangle();
            
            if (modeColor)
            {
                    rectangle.Width = 35;
                    rectangle.Height = 35;
                    rectangle.StrokeThickness = 1;
                    rectangle.Stroke = Brushes.White;
                    rectangle.Fill = getGradientColor(rectColor); //DAR COLOR A LAS FICHAS
                    mode.Content = "MODO LÍNEA";
                    return rectangle;
            }
            else 
            {
                rectangle.Width = 35;
                rectangle.Height = 35;
                rectangle.StrokeThickness = 3;
                rectangle.Stroke = Brushes.Blue;               
                mode.Content = "MODO COLOR";
                return rectangle;
            }
        }
        private LinearGradientBrush getGradientColor(Color clr)
        {
            LinearGradientBrush gradientColor = new LinearGradientBrush();
            gradientColor.StartPoint = new Point(0, 0);
            gradientColor.EndPoint = new Point(1, 1.5);
            GradientStop black = new GradientStop();
            black.Color = Colors.Black;
            black.Offset = -1.5;
            gradientColor.GradientStops.Add(black);
            GradientStop other = new GradientStop();
            other.Color = clr;
            other.Offset = 0.70;
            gradientColor.GradientStops.Add(other);
            return gradientColor;
        }

        private void reset()
        {
            downPos = 0;
            leftPos = 3;
            isRotated = false;
            rotation = 0;
            currentShapeNumber = nextShapeNumber;
            if (!isGameOver) { shapeAdd(currentShapeNumber, leftPos); }
            nextShapeDrawed = false;
            shapeRandom = new Random();
            nextShapeNumber = shapeRandom.Next(1, 8);
            bottomCollided = false;
            leftCollided = false;
            rightCollided = false;
        }
        private void shapeStoped()
        {
            timer.Stop();
            if (downPos <= 2)  // CONDICION PARA PERDER
            {
                gameOver();
                return;
            }

            int index = 0;
            while (index < Tetris.Children.Count)
            {
                UIElement element = Tetris.Children[index];
                if (element is Rectangle)
                {
                    Rectangle square = (Rectangle)element;
                    if (square.Name.IndexOf("moving_") == 0)
                    {
                        // Cambia el nombre de las figuras que van llegando
                        string newName = square.Name.Replace("moving_", "arrived_");
                        square.Name = newName;
                    }
                }
                index++;
            }
            // REVISA QUE LA LÍNEA REALMENTE ESTÉ COMPLETA
            // DESPUÉS DE QUE ESTÉ COMPLETA LA ELIMINA
            // LUEGO BAJA EL RESTO DE FIGURAS
            checkComplete();
            reset();
            timer.Start();

        }

        private void gameOver() 
        {
            isGameOver = true;
            reset();
            start.Content = "Empezar";
            GameOver.Visibility = Visibility.Visible;
            rowCount = 0;
            columnCount = 0;
            leftPos = 0;
            gameSpeedCounter = 0;
            gameSpeed = GAMESPEED;
            gameLevel = 1;
            gameActive = false;
            gameScore = 0;
            nextShapeDrawed = false;
            currentShape = null;
            currentShapeNumber = shapeRandom.Next(1, 8);
            nextShapeNumber = shapeRandom.Next(1, 8);
            timer.Interval = new TimeSpan(0, 0, 0, 0, gameSpeed);

        }
        private void deleteLine(int row)
        {
            //ELIMINA UNA LÍNEA CUANDO SE COMPLETA
            for (int i = 0; i < Tetris.ColumnDefinitions.Count; i++)
            {
                Rectangle square;
                try
                {
                    square = (Rectangle)Tetris.Children
                   .Cast<UIElement>()
                   .FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == i);
                    Tetris.Children.Remove(square);
                }
                catch { }

            }
            // BAJA EL RESTO DE FIGURAS QUE ESTÁN ENCIMA
            foreach (UIElement element in Tetris.Children)
            {
                Rectangle square = (Rectangle)element;
                if (square.Name.IndexOf("arrived") == 0 && Grid.GetRow(square) <= row)
                {
                    Grid.SetRow(square, Grid.GetRow(square) + 1);
                }
            }
        }
        private int getScore()
        {
            gameScore += 60 * gameLevel;
            return gameScore;
        }

        private void mode_Click(object sender, RoutedEventArgs e)
        {
            if (modeColor) {
                modeColor = false;
            }
            else
            {
                modeColor = true;
            }
        }

    }
}
