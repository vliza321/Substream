using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update

    public List<List<StageNode>> stages;
    public int currentLevel;
    public StageNode currentStage;
    public int maxLevel;

    public void Initialize()
    {
        currentLevel = 0;
        stages = new List<List<StageNode>>();

        for (int i =0; i < maxLevel; i++)
        {
            List<StageNode> stageLevel = new List<StageNode>();
            int numOfNodes = Random.Range(1, 4);
            for (int j = 0; j < numOfNodes; j++)
            {
                StageNode stageNode = new StageNode();
                stageNode.Initialize(i,j);
                stageLevel.Add(stageNode);
            }
            stages.Add(stageLevel);
        }

        for (int i = 0; i < stages.Count; i++)
        {
            Debug.Log("Level " + i + " has " + stages[i].Count + " nodes.");
        }

        for (int i = 0; i < maxLevel - 1; i++)
        {
            var nextNodes = stages[i + 1];
            var currentNodes = stages[i];

            foreach (var nextNode in nextNodes)
            {
                int numberOfConnections = Random.Range(1, currentNodes.Count+1);
                
                List<int> currentNodeIndex = Enumerable.Range(0, currentNodes.Count).ToList();
                currentNodeIndex = currentNodeIndex.OrderBy(_ => Random.value).ToList();
                currentNodeIndex = currentNodeIndex.Take(numberOfConnections).ToList();

                foreach (var index in currentNodeIndex)
                {
                    currentNodes[index].nextNodes.Add(nextNode);
                    numberOfConnections--;
                }
            }

            foreach (var node in currentNodes)
            {
                if (node.nextNodes.Count == 0)
                {
                    int randomIndex = Random.Range(0, nextNodes.Count);
                    node.nextNodes.Add(nextNodes[randomIndex]);
                }
            }
        }

        for(int i = 0; i < stages.Count; i++)
        {
            for (int j = 0; j < stages[i].Count; j++)
            {
                for(int k = 0; k < stages[i][j].nextNodes.Count; k++)
                {
                    Debug.Log("Node " + j + " in Level " + i + " has " + stages[i][j].nextNodes[k].StageID + " next nodes.");
                }
            }
        }
    }

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
