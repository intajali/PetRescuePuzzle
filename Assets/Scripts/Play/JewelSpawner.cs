using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;


public class JewelSpawner : MonoBehaviour
{

    public static JewelSpawner spawn;
    public List<JewelData> jewelDataList;
    public GameObject[,] JewelGrib;

    public JewelObj[,] JewelGribScript;

    public GameObject JewelParent;

    public GameObject JewelObject;

    public List<Sprite> JewelSprite;

    public GameObject Star;

    public GameObject StarEffect;

    public GameObject JewelColor;

    public GameObject NoSelect;

    private const float BaseDistance = 1f;

    private GameObject ObjTmp;

    private JewelObj JewelScript;

    public List<GameObject>[] prespawnlist = new List<GameObject>[7];

    public List<Sprite> greenItems;
    public List<Sprite> blueItems;
    public List<Sprite> redItems;
    public List<Sprite> yellowItems;
    public List<Sprite> animeSprite;

    public List<JewelData> greenJewel;
    public List<JewelData> blueJewel;
    public List<JewelData> redJewel;
    public List<JewelData> yellowJewel;
    public List<JewelData> enemyJewel;

    public int animCount = 0;
    public int animRegerate = 0;

    public int enemySpawnCheckerCount = 20;
    public int gemsGenerated = 0;

    void Awake()
    {
        gemsGenerated = 0;
        spawn = this;

        for (int i = 0; i < 7; i++)
        {
            prespawnlist[i] = new List<GameObject>();
        }
    }

    public void JewelMapCreate(int[,] Map)
    {
        // CreateJewleSpriteList();
        CreateRandomJewelItemList();
        JewelGrib = new GameObject[7, 9];

        JewelGribScript = new JewelObj[7, 9];

        for (int x = 0; x < 7; x++)
        {
            int s = 0;
            for (int y = 0; y < 9; y++)
            {
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 4)
                    s = y;
            }
            for (int y = s; y < 9; y++)
            {
                if (Map[x, y] > 0)
                {
                    RJewelInstantiate(x, y);
                }
            }
        }

        while (!Supporter.sp.isNoMoreMove())
        {
            RemakeGrib();
            JewelMapCreate(Map);
        }
    }
    void RemakeGrib()
    {
        for (int x = 0; x < 7; x++)
            for (int y = 0; y < 9; y++)
                if (JewelGrib[x, y] != null && JewelGribScript[x, y] != GameController.action.JewelStar)
                {
                    Destroy(JewelGrib[x, y]);
                    JewelGribScript[x, y] = null;
                }

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < prespawnlist[i].Count; j++)
            {
                if (prespawnlist[i][j] != null)
                    Destroy(prespawnlist[i][j]);
            }
            prespawnlist[i].Clear();
        }

    }

    public void CreateJewleSpriteList()
    {
        JewelSprite.Clear();
        if (greenItems.Count > 0)
        {
            var greenIndex = Random.Range(0, greenItems.Count);
            JewelSprite.Add(greenItems[greenIndex]);
        }

        if (blueItems.Count > 0)
        {
            var blueIndex = Random.Range(0, blueItems.Count);
            JewelSprite.Add(blueItems[blueIndex]);
        }

        if (redItems.Count > 0)
        {
            var redIndex = Random.Range(0, redItems.Count);
            JewelSprite.Add(redItems[redIndex]);
        }

        if (yellowItems.Count > 0)
        {
            var yellowIndex = Random.Range(0, yellowItems.Count);
            JewelSprite.Add(yellowItems[yellowIndex]);
        }

        if (animeSprite.Count > 0)
        {
            animCount++;
            var animeIndex = Random.Range(0, animeSprite.Count);
            JewelSprite.Add(animeSprite[animeIndex]);
        }

    }

    public void CreateRandomJewelItemList()
    {
        var levelInfo = GameController.action.levelDataContainer.GetLevelInfo(PlayerInfo.MapPlayer.Level);
        if (levelInfo == null || levelInfo.petToCollect == null || levelInfo.petToCollect.Count == 0)
        {
            Debug.LogError($"Cannot spawn jewels: missing level data for level {PlayerInfo.MapPlayer.Level}");
            return;
        }

        int[] jewelTypeRequired = new int[levelInfo.petToCollect.Count];

        for (int i = 0; i < levelInfo.petToCollect.Count; i++)
        {
            jewelTypeRequired[i] = levelInfo.petToCollect[i].jewelType;
        }


        jewelDataList.Clear();
        if (greenJewel.Count > 0)
        {
            int greenIndex = -1;
            for (int t = 0; t < jewelTypeRequired.Length; t++)
            {
                for (int i = 0; i < greenJewel.Count; i++)
                {
                    if (greenJewel[i].jewelType == jewelTypeRequired[t])
                    {
                        greenIndex = i;
                        jewelDataList.Add(greenJewel[greenIndex]);
                        //break;
                    }
                }
            }
            if (greenIndex < 0)
            {
                greenIndex = Random.Range(0, greenJewel.Count);
                jewelDataList.Add(greenJewel[greenIndex]);
            }
            
        }

        if (blueJewel.Count > 0)
        {
            int blueIndex = -1;
            for (int t = 0; t < jewelTypeRequired.Length; t++)
            {
                for (int i = 0; i < blueJewel.Count; i++)
                {
                    if (blueJewel[i].jewelType == jewelTypeRequired[t])
                    {
                        blueIndex = i;
                        jewelDataList.Add(blueJewel[blueIndex]);
                    }
                }
            }
            if (blueIndex < 0)
            {
                blueIndex = Random.Range(0, blueJewel.Count);
                jewelDataList.Add(blueJewel[blueIndex]);
            }


            
        }

        if (redJewel.Count > 0)
        {
            int redIndex = -1;
            for (int t = 0; t < jewelTypeRequired.Length; t++)
            {
                for (int i = 0; i < redJewel.Count; i++)
                {
                    if (redJewel[i].jewelType == jewelTypeRequired[t])
                    {
                        redIndex = i;
                        jewelDataList.Add(redJewel[redIndex]);
                       
                    }
                }
            }
            if (redIndex < 0)
            {
                redIndex = Random.Range(0, redJewel.Count);
                jewelDataList.Add(redJewel[redIndex]);
            }
            
            
        }

        if (yellowJewel.Count > 0)
        {
            int yellowIndex = -1;
            for (int t = 0; t < jewelTypeRequired.Length; t++)
            {
                for (int i = 0; i < yellowJewel.Count; i++)
                {
                    if (yellowJewel[i].jewelType == jewelTypeRequired[t])
                    {
                        yellowIndex = i;
                        jewelDataList.Add(yellowJewel[yellowIndex]);
                       // break;
                    }
                }
            }
            if (yellowIndex < 0)
            {
                yellowIndex = Random.Range(0, yellowJewel.Count);
                jewelDataList.Add(yellowJewel[yellowIndex]);
            }
            
        }

        if (enemyJewel.Count > 0)
        {
            var index = 0;
            if (PlayerInfo.MapPlayer.Level > 10 && PlayerInfo.MapPlayer.Level <= 45)
            {
                index = 1;
            }
            else if (PlayerInfo.MapPlayer.Level > 45)
            {
                index = 2;
            }
            jewelDataList.Add(enemyJewel[index]);
        }
    }

    public IEnumerator Respawn()
    {
    loop:
        RemakeGrib();

        for (int x = 0; x < 7; x++)
        {
            int s = 0;
            for (int y = 0; y < 9; y++)
            {
                if (GribManager.cell.GribCellObj[x, y] != null && GribManager.cell.GribCellObj[x, y].cell.CellEffect == 4)
                    s = y;
            }
            for (int y = s; y < 9; y++)
            {
                if (GribManager.cell.Map[x, y] > 0 && JewelGribScript[x, y] == null)
                {
                    RJewelInstantiate(x, y);
                }
            }
        }

        while (!Supporter.sp.isNoMoreMove())
        {
            goto loop;
        }

        EnableAllJewel();
        yield return new WaitForSeconds(0.75f);
        Timer.timer.NoSelect.SetActive(false);
        Timer.timer.Nomove.SetActive(false);
    }

    public void EnableAllJewel()
    {
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (JewelGribScript[x, y] != null && JewelGribScript[x, y] != GameController.action.JewelStar)
                    JewelGribScript[x, y].JewelEnable();
            }
        }
    }

    public GameObject JewelInstantiate(int x, int y)
    {
        int r = 0;
        JewelData data;

        if (GameController.action.moveCount % 3 == 0 && !GameController.action.drop.isEnemyCreated && GameController.action.enemyCount <= GameController.action.MaxEnemyPerLevel)
        {
            int t = Random.Range(0, jewelDataList.Count);
            data = jewelDataList[t];
            r = data.jewelType;
            if (r == 111 || r == 222 || r == 333 || r == 444)
            {
                GameController.action.drop.isEnemyCreated = true;
                GameController.action.enemyCount++;
            }
        }
        else
        {
            int t = Random.Range(0, jewelDataList.Count - 1);
            data = jewelDataList[t];
            r = data.jewelType;
        }

        ObjTmp = (GameObject)Instantiate(JewelObject);
        JewelScript = ObjTmp.GetComponent<JewelObj>();
        ObjTmp.transform.SetParent(JewelParent.transform, false);
        ObjTmp.transform.localPosition = new Vector3(ObjTmp.transform.localPosition.x + x * BaseDistance, ObjTmp.transform.localPosition.y + y * BaseDistance);
        JewelGrib[x, y] = ObjTmp;
        JewelGribScript[x, y] = JewelScript;

        //if (PLayerInfo.MODE == 1)
        //    r = Random.Range(0, 6);
        //else
        //    r = Random.Range(0, 7);



        JewelScript.render.sprite = data.jewelSprite;
        JewelScript.jewel.JewelPosition = new Vector2(x, y);
        JewelScript.jewel.JewelType = r;

        return ObjTmp;
    }

    public GameObject JewelInstantiatebt(int x, int y)
    {
        GameObject tmp;
        tmp = (GameObject)Instantiate(JewelObject);
        JewelScript = tmp.GetComponent<JewelObj>();

        tmp.transform.SetParent(JewelParent.transform, false);
        JewelScript.render.enabled = true;
        JewelGrib[x, y] = ObjTmp;
        JewelGribScript[x, y] = JewelScript;

        int r = 0;

        if (PlayerInfo.MODE == 1)
            r = Random.Range(0, 6);
        else
            r = Random.Range(0, 7);
        JewelScript.render.sprite = JewelSprite[r];
        JewelScript.jewel.JewelPosition = new Vector2(x, 9);
        JewelScript.jewel.JewelType = r;
        JewelScript.jewel.JewelPower = 0;
        return tmp;
    }

    GameObject remakeJewel(GameObject obj, int x)
    {
        GameObject o = obj;
        Animation anim = ObjTmp.GetComponent<Animation>();
        if (anim.GetClipCount() > 0)
            anim.RemoveClip("Moveto");

        prespawnlist[x].RemoveAt(0);


        o.transform.GetChild(0).transform.localPosition = Vector3.zero;

        if (o.transform.GetChild(0).gameObject.transform.childCount > 0)
            Destroy(o.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);
        return o;
    }

    GameObject RJewelInstantiate(int x, int y)
    {
        ObjTmp = (GameObject)Instantiate(JewelObject);
        JewelScript = ObjTmp.GetComponent<JewelObj>();
        ObjTmp.transform.SetParent(JewelParent.transform, false);
        ObjTmp.transform.localPosition = new Vector3(x, y);
        ObjTmp.transform.GetChild(0).gameObject.transform.localScale = new Vector3(0, 0, 1);
        JewelGrib[x, y] = ObjTmp;
        JewelGribScript[x, y] = JewelScript;

        // int r = randomjewel(x, y);
        JewelData data = RandomJewel(x, y);
        JewelScript.render.sprite = data.jewelSprite;
        // JewelScript.render.sprite = JewelSprite[r];
        JewelScript.jewel.JewelPosition = new Vector2(x, y);
        JewelScript.jewel.JewelType = data.jewelType;
        // JewelScript.jewel.JewelType = r;

        return ObjTmp;
    }

    int randomjewel(int x, int y)
    {
        int r = -1;
        int dem = 0;
        JewelData data;

        while (true)
        {

            //if (PLayerInfo.MODE == 1)

            //    r = Random.Range(0, 7);   //    r = Random.Range(0, 6);
            //else
            if (GameController.action.jewelMatchedCount % 3 == 0)
            {
                int t = Random.Range(0, jewelDataList.Count);
                data = jewelDataList[t];
                r = data.jewelType;
            }
            else
            {
                int t = Random.Range(0, jewelDataList.Count - 1);
                data = jewelDataList[t];
                r = data.jewelType;
            }

            //r = Random.Range(0, JewelSprite.Count);

            if (r == 0)
            {
                if (animCount <= 3)
                {
                    animCount++;
                }
                else
                {
                    continue;
                }
            }


            if (x < 2 || JewelGribScript[x - 1, y] == null | JewelGribScript[x - 2, y] == null || r != JewelGribScript[x - 1, y].jewel.JewelType || r != JewelGribScript[x - 2, y].jewel.JewelType)
            {
                if (y < 2 || JewelGribScript[x, y - 1] == null || JewelGribScript[x, y - 2] == null || JewelGribScript[x, y - 1].jewel.JewelType != r || JewelGribScript[x, y - 2].jewel.JewelType != r)
                {
                    return r;
                }
            }
            dem++;
            if (dem > 100)
                return 0;
        }
    }

    JewelData RandomJewel(int x, int y)
    {
        int r = -1;
        int dem = 0;
        JewelData data;

        while (true)
        {

            //if (PLayerInfo.MODE == 1)

            //    r = Random.Range(0, 7);   //    r = Random.Range(0, 6);
            //else
            //if (PlayerInfo.MapPlayer.Level >= 5)
            //{
            //    int t = Random.Range(0, jewelDataList.Count);
            //    data = jewelDataList[t];
            //    r = data.jewelType;
            //}
            //else
            {
                int t = Random.Range(0, jewelDataList.Count - 1);
                data = jewelDataList[t];
                r = data.jewelType;
            }

            //r = Random.Range(0, JewelSprite.Count);

            if (r == 111 || r == 222 || r == 333 || r == 444)
            {
                if (animCount <= 3)
                {
                    animCount++;
                }
                else
                {
                    continue;
                }
            }


            if (x < 2 || JewelGribScript[x - 1, y] == null | JewelGribScript[x - 2, y] == null || r != JewelGribScript[x - 1, y].jewel.JewelType || r != JewelGribScript[x - 2, y].jewel.JewelType)
            {
                if (y < 2 || JewelGribScript[x, y - 1] == null || JewelGribScript[x, y - 2] == null || JewelGribScript[x, y - 1].jewel.JewelType != r || JewelGribScript[x, y - 2].jewel.JewelType != r)
                {
                    return data;
                }
            }
            dem++;
            if (dem > 100)
                return null;
        }
    }

    public IEnumerator remakeGrib()
    {
        NoSelect.SetActive(true);
        yield return new WaitForSeconds(1f);
    }

    public GameObject SpawnJewelPower(int type, int power, Vector3 pos)
    {
        GameObject tmp;
        int x = (int)pos.x;
        int y = (int)pos.y;
        if (JewelGrib[x, y] != null)
            Destroy(JewelGrib[x, y]);
        if (power == (int)PowerTypes.LASER_PISTOL)
        {
            tmp = (GameObject)Instantiate(JewelColor);
        }
        else
        {
            tmp = (GameObject)Instantiate(JewelObject);
        }

        JewelScript = tmp.GetComponent<JewelObj>();
        JewelScript.render.enabled = true;
        tmp.transform.SetParent(JewelParent.transform, false);
        tmp.transform.localPosition = new Vector3(x, y, pos.z);
        JewelGrib[x, y] = tmp;
        JewelGribScript[x, y] = JewelScript;
        if (type != 101)
        {
            //JewelScript.render.sprite = JewelSprite[type];
            var jData = jewelDataList.Where(t => t.jewelType == type).FirstOrDefault();
            if (jData != null)
            {
                JewelScript.render.sprite = jData.jewelSprite;
            }
        }
        JewelScript.jewel.JewelPosition = new Vector2(x, y);
        JewelScript.jewel.JewelType = type;
        JewelScript.jewel.JewelPower = power;
        tmp.GetComponent<Collider2D>().enabled = false;
        if (power == (int)PowerTypes.BOMB_X || power == (int)PowerTypes.POWERUP_COLUMN ||
            power == (int)PowerTypes.POWERUP_ROW)
        {
            GameController.action.PBoom(x, y);
            EffectSpawner.effect.Enchant(tmp.transform.GetChild(0).gameObject, power);
            JewelScript.render.sprite = null;
        }
        return tmp;
    }

    public void SpawnStar(Vector2 pos)
    {
        if (JewelGribScript[(int)pos.x, (int)pos.y] != null)
            Destroy(JewelGrib[(int)pos.x, (int)pos.y]);

        GameObject tmp = (GameObject)Instantiate(Star);
        tmp.name = "JewelStar";
        tmp.transform.SetParent(JewelParent.transform, false);
        tmp.transform.localPosition = new Vector3(pos.x, pos.y);
        tmp.transform.GetChild(0).gameObject.SetActive(false);
        JewelScript = tmp.GetComponent<JewelObj>();
        JewelScript.jewel.JewelPosition = pos;
        JewelGribScript[(int)pos.x, (int)pos.y] = JewelScript;
        JewelGrib[(int)pos.x, (int)pos.y] = tmp;
        GameController.action.JewelStar = JewelScript;

        StarEffect.SetActive(true);
    }

}

[System.Serializable]
public class JewelData
{
    public Sprite jewelSprite;
    public int jewelType;
}
