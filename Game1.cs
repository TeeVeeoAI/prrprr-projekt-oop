using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.States;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private State currentState;
    private State nextState;
    private static Vector2 screenSize;
    private static Rectangle enemySpawnArea;
    private string path;
    private Color BGColor;
    private FpsCounter fpsCounter;
    private SpriteFont debugFont;
    public static Rectangle EnemySpawnArea { get => enemySpawnArea; }
    public static Vector2 ScreenSize { get => screenSize; }
    public string Path { get => path; }
    public void SetBGColor(Color color) { BGColor = color; }

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        IsFixedTimeStep = false; // uncapped FPS
        _graphics.SynchronizeWithVerticalRetrace = true; //V-Sync
    }

    protected override void Initialize()
    {
        Window.TextInput += OnTextInput;

        base.Initialize();
    }

    private void OnTextInput(object sender, TextInputEventArgs e)
    {
        //Forward typed character to InputSystem for consumption by states
        InputSystem.QueueTypedChar(e.Character);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ChangeScreenSize(new Vector2(1920, 1080));
        enemySpawnArea = new Rectangle(0, -50, (int)screenSize.X, 50);
        path = GetFromNestedToRoot(Directory.GetCurrentDirectory());

        fpsCounter = new FpsCounter();
        debugFont = Content.Load<SpriteFont>("Fonts/MainFont");

        currentState = new MenuState(this, GraphicsDevice, Content);
        currentState.LoadContent();
    }

    private string GetFromNestedToRoot(string directory)
        {
            List<string> words = new List<string>(directory.Split('\\'));
            Console.WriteLine(directory);
            int i = 0;
            while (true)
            {
                if (words[words.Count - 1] != "OneUpSolverV3" && words.Count != 1)
                {
                    words.RemoveAt(words.Count - 1);
                    i++;
                }
                else
                {
                    for (int j = 0; j < i; j++)
                    {
                        directory = Directory.GetParent(directory).FullName;
                    }
                    break;
                }
            }
            return directory;
        }

    public void ChangeScreenSize(Vector2 newSize)
    {
        screenSize = newSize;
        _graphics.PreferredBackBufferWidth = (int)screenSize.X;
        _graphics.PreferredBackBufferHeight = (int)screenSize.Y;
        _graphics.ApplyChanges();
    }

    public void ChangeState(State state)
    {
        nextState = state;
        nextState.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (nextState != null)
        {
            currentState = nextState;
            nextState = null;
        }

        fpsCounter.Update(gameTime);
        currentState.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(BGColor);

        currentState.Draw(gameTime, _spriteBatch);

        _spriteBatch.Begin();
        fpsCounter.Draw(_spriteBatch, debugFont, new Vector2(10, screenSize.Y - 40), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
