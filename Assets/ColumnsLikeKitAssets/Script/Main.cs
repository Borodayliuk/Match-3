using UnityEngine;
using System.Collections;
using Holoville.HOTween;

/// <summary>
///  This class is the main entry point of the game it should be attached to a gameobject and be instanciate in the scene
/// Author : Pondomaniac Games
/// </summary>
public class MainGame : MonoBehaviour
{
    public GameObject[,] _arrayOfShapes;//The main array that contain all games tiles
    public GameObject[] _listOfGems;//The list of tiles we cant to see in the game you can remplace them in unity's inspector and choose all what you want
    public GameObject _particleEffect;//The object we want to use in the effect of shining stars 
    public GameObject _particleEffectWhenMatch;//The gameobject of the effect when the objects are matching
    public int _scoreIncrement;//The amount of point to increment each time we find matching tiles
    private int _scoreTotal = 0;//The score 
    private ArrayList _currentParticleEffets = new ArrayList();//the array that will contain all the matching particle that we will destroy after
    public AudioClip MatchSound;//the sound effect when matched tiles are found
    public int _gridWidth;//the grid number of cell horizontally
    public int _gridHeight;//the grid number of cell vertically
    
    //inside class
    private GameObject _newShape1;//The new created shape
    private GameObject _newShape2;//The new created shape
    private GameObject _newShape3;//The new created shape
    private Vector2 _firstPressPos;
    private Vector2 _secondPressPos;
    private Vector2 _currentSwipe;
    private GameObject _nextNewShape1;//The new created shape
    private GameObject _nextNewShape2;//The new created shape
    private GameObject _nextNewShape3;//The new created shape
    private GameStateManager gamestateinstance;
    // Use this for initialization
    void Awake()
    {   //Adding the star effect to the gems and call the DoShapeEffect continuously
        InvokeRepeating("DoShapeEffect", 1f, 1F);
       gamestateinstance = new GameStateManager();
        //Initializing the array with _gridWidth and _gridHeight passed in parameter
        _arrayOfShapes = new GameObject[_gridWidth, _gridHeight];

        _nextNewShape1 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth , _gridHeight - 2, 0), transform.rotation) as GameObject;
        _nextNewShape2 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth , _gridHeight - 1, 0), transform.rotation) as GameObject;
        _nextNewShape3 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth , _gridHeight, 0), transform.rotation) as GameObject;

        //StartByCreating a new shape
        CreateANewShape();
       

    }
    void CreateANewShape()
    {
        _newShape1 = _nextNewShape1; 
        _newShape2 = _nextNewShape2;
        _newShape3 = _nextNewShape3;

        _nextNewShape1 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth +2 , _gridHeight-5, -1), transform.rotation) as GameObject;
        _nextNewShape2 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth +2, _gridHeight-4, -1), transform.rotation) as GameObject;
        _nextNewShape3 = GameObject.Instantiate(_listOfGems[Random.Range(0, _listOfGems.Length)] as GameObject, new Vector3(_gridWidth +2, _gridHeight-3, -1), transform.rotation) as GameObject;
        
        _newShape1.transform.localPosition = new Vector3(_gridWidth / 2, _gridHeight - 2, 0);
        _newShape2.transform.localPosition= new Vector3(_gridWidth / 2, _gridHeight - 1, 0);
        _newShape3.transform.localPosition= new Vector3(_gridWidth / 2, _gridHeight , 0);

    }

    //Instantiate the star objects
    void GoDown()
    {

             //float xFactor = 0.01f;

             float xFactor = 0.01f *( GameStateManager.Level  );
            _newShape1.transform.localPosition = new Vector2(_newShape1.transform.localPosition.x, _newShape1.transform.localPosition.y - xFactor);
            _newShape2.transform.localPosition = new Vector2(_newShape2.transform.localPosition.x, _newShape2.transform.localPosition.y - xFactor);
            _newShape3.transform.localPosition = new Vector2(_newShape3.transform.localPosition.x, _newShape3.transform.localPosition.y - xFactor);

    }
    void GoDownAnimation()
    {
        int Yposition = GetTheBottomYPosition();
       
        if (Yposition > -1 && HOTween.GetTweenInfos() == null)
        {
            TweenParms parms = new TweenParms().Prop("position", new Vector3((int)_newShape1.transform.localPosition.x, (int)Yposition , -1)).Ease(EaseType.EaseOutQuart);
            HOTween.To(_newShape1.transform, .4f, parms);
             parms = new TweenParms().Prop("position", new Vector3((int)_newShape2.transform.localPosition.x, (int)Yposition +1, -1)).Ease(EaseType.EaseOutQuart);
            HOTween.To(_newShape2.transform, .4f, parms);
             parms = new TweenParms().Prop("position", new Vector3((int)_newShape3.transform.localPosition.x, (int)Yposition + 2, -1)).Ease(EaseType.EaseOutQuart);
            HOTween.To(_newShape3.transform, .4f, parms);

        }


    }
    void GoRight()
    {

        checkAndGoRight(_newShape1);
        checkAndGoRight(_newShape2);
        checkAndGoRight(_newShape3);
    }

    void checkAndGoRight(GameObject go)
    {
        if (go.transform.localPosition.x < _gridWidth - 1)
        {
            var cell = _arrayOfShapes[(int)go.transform.localPosition.x + 1, (int)go.transform.localPosition.y];
            if (cell == null)
            {
                TweenParms parms = new TweenParms().Prop("position", new Vector3((int)go.transform.localPosition.x + 1, go.transform.localPosition.y, -1)).Ease(EaseType.EaseOutQuart);
                HOTween.To(go.transform, .1f, parms);
            }
        }

    }
    void GoLeft()
    {
        checkAndGoLeft(_newShape1);
        checkAndGoLeft(_newShape2);
        checkAndGoLeft(_newShape3);
    }

    void checkAndGoLeft(GameObject go)
    {
        if (go.transform.localPosition.x > 0)
        {
            var cell = _arrayOfShapes[(int)go.transform.localPosition.x - 1, (int)go.transform.localPosition.y];
            if (cell == null)
            {
                TweenParms parms = new TweenParms().Prop("position", new Vector3((int)go.transform.localPosition.x - 1, go.transform.localPosition.y, -1)).Ease(EaseType.EaseOutQuart);
                HOTween.To(go.transform, .1f, parms);
            }
        }
    }
    bool CanGo(Direction direction)
    {
        int postion = 0;
        switch (direction)
        {
            case Direction.LEFT:
                postion = -1;
                break;
            case Direction.RIGHT:
                postion = 1;
                break;
            default:
                break;
        }


        if ((_newShape1.transform.localPosition.x + postion < _gridWidth && _newShape1.transform.localPosition.x + postion >=0) && _arrayOfShapes[(int)_newShape1.transform.localPosition.x + postion, (int)_newShape1.transform.localPosition.y] == null
             && _arrayOfShapes[(int)_newShape2.transform.localPosition.x + postion, (int)_newShape2.transform.localPosition.y] == null
             && _arrayOfShapes[(int)_newShape3.transform.localPosition.x + postion, (int)_newShape3.transform.localPosition.y] == null)
        {


            return true;

        }
        return false;
    }



    void Shuffle()
    {
       
        TweenParms parms = new TweenParms().Prop("position", _newShape1.transform.position).Ease(EaseType.EaseOutQuart);
        HOTween.To(_newShape2.transform, .2f, parms);

        parms = new TweenParms().Prop("position", _newShape2.transform.position).Ease(EaseType.EaseOutQuart);
        HOTween.To(_newShape3.transform, .2f, parms);

       parms = new TweenParms().Prop("position", _newShape3.transform.position).Ease(EaseType.EaseOutQuart);
        HOTween.To(_newShape1.transform, .2f, parms);

        GameObject go = _newShape1;
        _newShape1 = _newShape2;
        _newShape2 = _newShape3;
        _newShape3 = go;

    }

    void DrawLine(Vector3 start, Vector3 end, float duration = 0.2f)
    {
        

      GameObject lineObj = new GameObject("DragLine", typeof(LineRenderer));
     LineRenderer line = lineObj.GetComponent<LineRenderer>();
     line.SetWidth(0.03f, 0.03f);
     line.SetColors(Color.white, Color.white);
     line.SetPosition(0, start);
     line.SetPosition(1, end);
    // Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
    // line.material = whiteDiffuseMat;

        GameObject.Destroy(lineObj , duration);
    }

    void Update()
    {Debug.Log("paused? " + GameStateManager.IsGamePaused);
        if (GameStateManager.IsGamePaused == false)
        {
            DrawLine(new Vector3(_newShape1.transform.localPosition.x, _newShape1.transform.localPosition.y - 1f, -1), new Vector3(_newShape1.transform.localPosition.x, 0, -1), 0.02f);


            if (HOTween.GetTweenInfos() == null )
            {
              
                var Matches = FindMatch(_arrayOfShapes);
               
                //If we find a matched tiles
                if (Matches.Count > 0)
                {//Update the score
                  _scoreTotal += Matches.Count * _scoreIncrement;
                    //Debug.Log("Matches count :" + Matches.Count);
                    foreach (GameObject go in Matches)
                    {

                        //Playing the matching sound
                        GetComponent<AudioSource>().PlayOneShot(MatchSound);
                        //Creating and destroying the effect of matching
                        var destroyingParticle = GameObject.Instantiate(_particleEffectWhenMatch as GameObject, new Vector3(go.transform.position.x, go.transform.position.y, -2), transform.rotation) as GameObject;
                        Destroy(destroyingParticle, 1f);


                        //Replace the matching tile with an empty one
                        _arrayOfShapes[(int)go.transform.position.x, (int)go.transform.position.y] = null;
                        //Destroy the ancient matching tiles
                        Destroy(go, 0.1f);

                    }

                    DoEmptyDown(ref _arrayOfShapes);
                }else 
                {

                     var direction = Swipe();

                if ((Input.GetKeyDown(KeyCode.RightArrow) || direction == Direction.RIGHT) && CanGo(Direction.RIGHT))
                {
                    GoRight();
                }
                else if ((Input.GetKeyDown(KeyCode.LeftArrow) || direction == Direction.LEFT) && CanGo(Direction.LEFT))
                {
                    GoLeft();
                }
                else if (Input.GetKey(KeyCode.DownArrow) || direction == Direction.DOWN)
                {
                    GoDownAnimation();
                }
                else if (Input.GetKey(KeyCode.Space) || direction == Direction.STATIONARY) { Shuffle(); }

                //Vertical
                int Yposition = GetTheBottomYPosition();
                if (Yposition >= _gridHeight - 2)
                {
                    //Game Over
                   // (GetComponent(typeof(TextMesh)) as TextMesh).text = "game over";
                    GameStateManager.IsGameOver = true;
                    return;

                }
                else if (_newShape1.transform.localPosition.y <= (float)Yposition)
                {//My  object reached the bottom I need to add it to the table 
                    _arrayOfShapes[(int)_newShape1.transform.localPosition.x, Yposition] = _newShape1;
                    _arrayOfShapes[(int)_newShape2.transform.localPosition.x, Yposition + 1] = _newShape2;
                    _arrayOfShapes[(int)_newShape3.transform.localPosition.x, Yposition + 2] = _newShape3;
                    CreateANewShape();
                }
                else
                { Debug.Log("go down " );

                    GoDown();
                }

                }

            }

    //Update the score
    GameStateManager.Score = _scoreTotal;
   // (GetComponent(typeof(TextMesh)) as TextMesh).text = _scoreTotal.ToString();
        }
    }

    
      
    int GetTheBottomYPosition()
    {

        int upper = _arrayOfShapes.GetUpperBound(1);
        int lower = _arrayOfShapes.GetLowerBound(1);
    
        for  (int y = upper; y >= lower; y-- ) {
            if (_arrayOfShapes[(int)_newShape1.transform.localPosition.x, y] != null  && y >= 0)
            {
                 return y +1 ;

            }
        }  
        return 0;
         }
   
    // Find Matching  Tiles
    private ArrayList FindMatch(GameObject[,] cells)
    {//creating an arraylist to store the matching tiles
        ArrayList stack = new ArrayList();
        //Checking the vertical tiles
        for (var x = 0; x <= cells.GetUpperBound(0); x++)
        {
            for (var y = 0; y <= cells.GetUpperBound(1); y++)
            {
                var thiscell = cells[x, y];
                if (thiscell != null)
                {     //If it's an empty tile continue
                    if (thiscell.name == "Empty(Clone)") continue;
                int matchCount = 0;
                int y2 = cells.GetUpperBound(1);
                int y1;
                //Getting the number of tiles of the same kind
                for (y1 = y + 1; y1 <= y2; y1++)
                {
                    if (cells[x, y1] == null || cells[x, y1].name == "Empty(Clone)" || thiscell.name != cells[x, y1].name) break;
                    matchCount++;
                }
                //If we found more than 2 tiles close we add them in the array of matching tiles
                if (matchCount >= 2)
                {
                    y1 = Mathf.Min(cells.GetUpperBound(1), y1 - 1);
                    for (var y3 = y; y3 <= y1; y3++)
                    {
                        if (!stack.Contains(cells[x, y3]))
                        {
                            stack.Add(cells[x, y3]);
                        }
                    }
                }

            }
            }
        }
        //Checking the horizontal tiles , in the following loops we will use the same concept as the previous ones
        for (var y = 0; y < cells.GetUpperBound(1) + 1; y++)
        {
            for (var x = 0; x < cells.GetUpperBound(0) + 1; x++)
            {
                var thiscell = cells[x, y];
                if (thiscell != null)
                {
                 
                    if (thiscell.name == "Empty(Clone)") continue;
                    int matchCount = 0;
                    int x2 = cells.GetUpperBound(0);
                    int x1;
                    for (x1 = x + 1; x1 <= x2; x1++)
                    {
                        if (cells[x1, y] == null ||  cells[x1, y].name == "Empty(Clone)" || thiscell.name != cells[x1, y].name) break;
                        matchCount++;
                    }
                    if (matchCount >= 2)
                    {
                        x1 = Mathf.Min(cells.GetUpperBound(0), x1 - 1);
                        for (var x3 = x; x3 <= x1; x3++)
                        {
                            if (!stack.Contains(cells[x3, y]))
                            {
                                stack.Add(cells[x3, y]);
                            }
                        }
                    }
                }
            }
        }
        return stack;
    }

    // Swap Motion Animation, to animate the switching arrays
    void DoSwapMotion(Transform a, Transform b)
    {
        Vector3 posA = a.localPosition;
        Vector3 posB = b.localPosition;
        TweenParms parms = new TweenParms().Prop("localPosition", posB).Ease(EaseType.EaseOutQuart);
        HOTween.To(a, 0.25f, parms).WaitForCompletion();
        parms = new TweenParms().Prop("localPosition", posA).Ease(EaseType.EaseOutQuart);
        HOTween.To(b, 0.25f, parms).WaitForCompletion();
    }


    // Swap Two Tile, it swaps the position of two objects in the grid array
    void DoSwapTile(GameObject a, GameObject b, ref GameObject[,] cells)
    {
        GameObject cell = cells[(int)a.transform.position.x, (int)a.transform.position.y];
        cells[(int)a.transform.position.x, (int)a.transform.position.y] = cells[(int)b.transform.position.x, (int)b.transform.position.y];
        cells[(int)b.transform.position.x, (int)b.transform.position.y] = cell;
    }

    // Do Empty Tile Move Down
    private void DoEmptyDown(ref GameObject[,] cells)
    {   //replace the empty tiles with the ones above
         for (int x = 0; x <= cells.GetUpperBound(0); x++)
          {
              for (int y = 0; y <= cells.GetUpperBound(1); y++)
              {

                  var thisCell = cells[x, y];
                  if (thisCell == null || thisCell.name == "Empty(Clone)")
                  {

                      for (int y2 = y; y2 <= cells.GetUpperBound(1); y2++)
                      {
                          if (cells[x, y2] != null &&  cells[x, y2].name != "Empty(Clone)")
                          {
                              cells[x, y] = cells[x, y2];
                              cells[x, y2] = thisCell;
                              break;
                          }

                      }

                  }

              }
          }


          for (int x = 0; x <= cells.GetUpperBound(0); x++)
          {
              for (int y = 0; y <= cells.GetUpperBound(1); y++)
              {
                  if (cells[x, y] != null)
                  {
                      TweenParms parms = new TweenParms().Prop("position", new Vector3(x, y, -1)).Ease(EaseType.EaseOutQuart);
                      HOTween.To(cells[x, y].transform, .4f, parms);
                  }
              }
          }
  
    }
    //Instantiate the star objects
    void DoShapeEffect()
    {
        foreach (GameObject row in _currentParticleEffets)
            Destroy(row);
        for (int i = 0; i <= 2; i++)
        {

            int x = Random.Range(0, _arrayOfShapes.GetUpperBound(0) + 1);
            int y = Random.Range(0, _arrayOfShapes.GetUpperBound(1) + 1);
            if (_arrayOfShapes[x,y] != null )
            _currentParticleEffets.Add(GameObject.Instantiate(_particleEffect, new Vector3(x,y, -1), new Quaternion(0, 0, Random.Range(0, 1000f), 100)) as GameObject);
        }
        }


    enum Direction
    {       NONE,
            STATIONARY,
            UP,
            DOWN,
            LEFT,
            RIGHT,
    }

    private Direction Swipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

            //normalize the 2d vector
            _currentSwipe.Normalize();

            //swipe upwards
            if (_currentSwipe.y > 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
        {
              //  Debug.Log("up swipe");
                return Direction.UP;
            }
            //swipe down
            if (_currentSwipe.y < 0 && _currentSwipe.x > -0.5f && _currentSwipe.x < 0.5f)
        {
               // Debug.Log("down swipe");
                return Direction.DOWN;
            }
            //swipe left
            if (_currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
        {
               // Debug.Log("left swipe");
                return Direction.LEFT;
            }
            //swipe right
            if (_currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f)
        {
               // Debug.Log("right swipe");
                return Direction.RIGHT;
            }
           // Debug.Log("stationnary swipe");
            return Direction.STATIONARY;
        }
        return Direction.NONE;
    }
}
