using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMines : MonoBehaviour
{
    public List<GameObject> blocks = new List<GameObject>();
    public List<GameObject> rows = new List<GameObject>();

    public GameObject crystal;
    public GameObject meteor;

    private void Start()
    {
        float x = -17;
        float y = 1.5f;
        float lastX = 124;

        float currentX = x;
        for (int i = 0; i < rows.Count; i++)
        {
            int index = Random.Range(1, 37);

            int indexMeteor1 = Random.Range(1, 37);
            int indexMeteor2 = Random.Range(1, 37);
            int indexMeteor3 = Random.Range(1, 37);

            int num=0;
            while (currentX <= lastX)
            {
                GameObject newBlock = Instantiate(blocks[Random.Range(0, blocks.Count)], rows[i].transform);
                newBlock.transform.localPosition = new Vector3(currentX, y, 0);
                currentX += 3;

                bool alreadyPlaced = false;

                if(num == index)
                {
                    GameObject newCrystal = Instantiate(crystal, rows[i].transform);
                    newCrystal.transform.localPosition = new Vector3(currentX, y, 0);

                    alreadyPlaced = true;
                }

                if((num == indexMeteor1 || num == indexMeteor2 || num == indexMeteor3) && !alreadyPlaced)
                {
                    GameObject newMeteor = Instantiate(meteor, rows[i].transform);
                    newMeteor.transform.localPosition = new Vector3(currentX, y, 0);
                }

                num++;
            }

            currentX = x;
        }
    }
}
