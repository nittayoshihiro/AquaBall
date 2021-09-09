using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeedTest : MonoBehaviour
{
    RandomSeed m_randomSeed = default;

    int m_saiki = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_randomSeed = new RandomSeed(9);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RandomSeed3()
    {
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(i + ":" + m_randomSeed.Range(1, 10));
        }
    }

    public void RandomSeedNum(int seed)
    {
        m_randomSeed.SetSeed(seed);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(i + ":" + m_randomSeed.Range(1, 10));
        }
    }

    public void SaikiTest()
    {
        if (m_saiki < 3)
        {
            RandomSeed3();
            m_saiki++;
            SaikiTest();
        }
    }
}
