using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Fase1 : MonoBehaviour
{
    public GameObject defaultTarget;
    
    public string Grammar;
    
    private float DistanceIntoTargets = 3f;
    
    private Quaternion defaultQuartenion = Quaternion.Euler(-90, -180, 0);
    
    private Vector3 defaultVector3 = new Vector3(-3f, 4.45f, 0f);
    
    private GameObject[] rules;

    private GameObject[] headerRules;
    
    private GameObject parentHeaderRules;

    private GameObject currentRule;

    private int currentIndex;

    [SerializeField] Text GrammarVisible;

    private int Step = 0;

    private bool setupStep = false;

    // Start is called before the first frame update
    void Start()
    {
        ParseGramar();
        GrammarVisible.text = GramarToString();
    }

    // Update is called once per frame
    void Update()
    {
        //Step 1 - Selecionar uma regra para ser alterada
        if(Step == 0)
        {
            if(!setupStep)
            {
                SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = IsSomeHeaderSelected();
            if(indexHeader != -1)
            {
                SetHeaderRuleInactive();
                SetRuleActive(indexHeader);
            }
        }
        //Step 2 - Selecionar uma variavel para ser alterada

        //Step 3 - Selecionar quais as variaveis dentro desta nova variavel

        //Step 4 - Identificar aonde remover

    }

    void ParseGramar()
    {
        rules = new GameObject[Grammar.Split(',').Length];
        headerRules = new GameObject[Grammar.Split(',').Length];
        var objectHeaderRule = new GameObject();
        objectHeaderRule.name = "TargetHeaders";
        objectHeaderRule.transform.parent = gameObject.transform;
        parentHeaderRules = objectHeaderRule;
        for (int j = 0; j < Grammar.Split(',').Length; j++)
        {
            string[] chars = Grammar.Split(',')[j].Replace(" ", "").Split(new string[] { "->" }, StringSplitOptions.None);
            var objectRule = new GameObject();
            objectRule.transform.parent = gameObject.transform;
            objectRule.name = chars[0];
            objectRule.SetActive(false);
            rules[j] = objectRule;
            
            headerRules[j] = Instantiate(defaultTarget, objectHeaderRule.transform);
            headerRules[j].SetActive(true);
            headerRules[j].transform.rotation = defaultQuartenion;
            headerRules[j].transform.position = new Vector3(defaultVector3.x + (DistanceIntoTargets * j), defaultVector3.y, defaultVector3.z);
            headerRules[j].name = chars[0].ToString();
            headerRules[j].GetComponent<TargetMovimentation>().Symbol = chars[0].ToString();

            for (int i = 0; i < chars[1].Length; i++)
            {
                var clone = Instantiate(defaultTarget, objectRule.transform);
                clone.SetActive(true);
                clone.transform.rotation = defaultQuartenion;
                clone.transform.position = new Vector3(defaultVector3.x+(DistanceIntoTargets * i), defaultVector3.y, defaultVector3.z);
                clone.name = chars[1][i].ToString();
                clone.GetComponent<TargetMovimentation>().Symbol = chars[1][i].ToString();
            }
        }
    }

    void SetRuleActive(int index)
    {
        SetRuleInactive();
        SetHeaderRuleInactive();

        rules[index].SetActive(true);
        currentIndex = index;
        currentRule = rules[index];
    }

    void SetHeaderRuleActive()
    {
        parentHeaderRules.SetActive(true);
    }

    int IsSomeHeaderSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            if (headerRules[i].GetComponent<TargetMovimentation>().selected)
            {
                return i;
            }
        }
        return -1;
    }


    void SetHeaderRuleInactive()
    {
        parentHeaderRules.SetActive(false);
    }

    void SetRuleInactive()
    {
        if(rules[currentIndex])
        {
            rules[currentIndex].SetActive(false);
        }
    }

    string GramarToString()
    {
        string FullGrammar = "";
        string[] text = Grammar.Split(',');
        foreach (var grammar in text)
        {
            FullGrammar = $"{FullGrammar}\n {grammar}";
        }
        return FullGrammar;
    }
}
