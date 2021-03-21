﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Grammar : MonoBehaviour
{
    [SerializeField] Text GrammarVisible;

    public string StringGrammar;
    // Game Object having all header rules
    private GameObject parentHeaderRules;
    // Object into Fase with all rules, used to select when hit into a header
    private GameObject[] rules;
    // Is Game Object of headers to hit
    private GameObject[] headerRules;
    // Inside of rules has N symbolRules is the terminal symbol
    private List<List<GameObject>> symbolRules;
    
    private Quaternion defaultQuartenion = Quaternion.Euler(-90, -180, 0);

    private Vector3 defaultVector3 = new Vector3(-3f, 3.45f, 0f);

    public GameObject defaultTarget;

    private float DistanceIntoTargets = 3f;

    public string newRuleName = "C";

    void DestroyRules()
    {
        if (rules == null)
        {
            return;
        }
        foreach (GameObject rule in rules)
        {
            Destroy(rule);
        }
    }

    void DestroyParentHeaderRules()
    {
        if (parentHeaderRules == null)
        {
            return;
        }
        Destroy(parentHeaderRules);
    }

    void DestroyHeaderRules()
    {
        if (headerRules == null)
        {
            return;
        }
        foreach (GameObject headerRule in headerRules)
        {
            Destroy(headerRule);
        }
    }

    void DestroySymbolRules()
    {
        if (symbolRules == null)
        {
            return;
        }

        foreach (List<GameObject> symbolRule in symbolRules)
        {
            foreach (GameObject rule in symbolRule)
            {
                Destroy(rule);
            }
        }
    }

    GameObject CreateEmptyObject(Transform transform, string name, bool active = true)
    {
        var obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = name;
        obj.SetActive(active);
        return obj;
    }



    GameObject CreateDefaultObject(Transform transform, string name, Vector3 position, bool active = true)
    {
        var obj = Instantiate(defaultTarget, transform);
        obj.SetActive(active);
        obj.transform.rotation = defaultQuartenion;
        obj.transform.position = position;
        obj.name = name;
        obj.GetComponent<TargetMovimentation>().Symbol = name;
        return obj;
    }

    public void ParseGramar()
    {
        DestroyParentHeaderRules();
        DestroyRules();
        DestroyHeaderRules();
        DestroySymbolRules();
        var gramarSplitComma = StringGrammar.Split(',');
        rules = new GameObject[gramarSplitComma.Length];
        headerRules = new GameObject[gramarSplitComma.Length];
        symbolRules = new List<List<GameObject>>();
        parentHeaderRules = CreateEmptyObject(
            gameObject.transform,
            "TargetHeaders",
            false
        );
        for (int j = 0; j < gramarSplitComma.Length; j++)
        {
            string[] chars = gramarSplitComma[j].Replace(" ", "").Split(new string[] { "->" }, StringSplitOptions.None);

            rules[j] = CreateEmptyObject(transform, chars[0], false);

            headerRules[j] = CreateDefaultObject(
                parentHeaderRules.transform,
                chars[0].ToString(),
                new Vector3(defaultVector3.x + (DistanceIntoTargets * j), defaultVector3.y, defaultVector3.z)
                );

            List<GameObject> ruleJ = new List<GameObject>();

            for (int i = 0; i < chars[1].Length; i++)
            {
                ruleJ.Add(
                    CreateDefaultObject(
                    rules[j].transform,
                    chars[1][i].ToString(),
                    new Vector3(
                        defaultVector3.x + (DistanceIntoTargets * i),
                        defaultVector3.y,
                        defaultVector3.z
                        )
                    )
                );
            }
            symbolRules.Add(ruleJ);
        }
    }

    public GameObject FindSymbolRulesByHeaderAndName(int header, string name)
    {
        return symbolRules[header].Find(x => x.name == name);
    }
    public void SetRuleNotSelected(int index)
    {
        foreach (GameObject rule in symbolRules[index])
        {
            rule.GetComponent<TargetMovimentation>().selected = false;
        }
    }

    public void SetTargetRulesActiveAndNotSelectedAndFixPosition(int index)
    {
        foreach (GameObject rule in symbolRules[index])
        {
            rule.SetActive(true);
            rule.GetComponent<TargetMovimentation>().selected = false;
            rule.transform.position = new Vector3(rule.transform.position.x, rule.transform.position.y, defaultTarget.transform.position.z);
        }
    }

    public void replaceTwoRulesByDefaultRule(GameObject rule1, GameObject rule2, int headerIndex)
    {
        int index = symbolRules[headerIndex].FindIndex(x => x == rule1);
        int index2 = symbolRules[headerIndex].FindIndex(x => x == rule2);
        symbolRules[headerIndex].Insert(
            Math.Min(index, index2),
            CreateDefaultObject(
                rules[headerIndex].transform,
                newRuleName,
                defaultVector3,
                false
            )
        );
        List<GameObject> objectsToDelete = new List<GameObject>();
        objectsToDelete.Add(rule1);
        objectsToDelete.Add(rule2);
        deleteVisibleRule(headerIndex, objectsToDelete);
    }

    void deleteVisibleRule(int indexRule, List<GameObject> rules)
    {
        foreach (GameObject rule in rules)
        {
            GameObject findRule = symbolRules[indexRule].Find(x => x == rule);
            Destroy(findRule);
            symbolRules[indexRule].Remove(rule);
        }

        FixDistanceRule(indexRule);
        UpdateVisibleGramar();
    }


    void FixDistanceRule(int index)
    {
        int i = 1;
        foreach (GameObject rule in symbolRules[index])
        {
            rule.transform.position = new Vector3(defaultVector3.x + (DistanceIntoTargets * i), defaultVector3.y, defaultVector3.z);
            i++;
        }
    }



    string GramarToString()
    {
        string FullGrammar = "Gramática";
        for (int i = 0; i < headerRules.Length; i++)
        {
            FullGrammar = $"{FullGrammar}\n {headerRules[i].name} -> ";
            foreach (GameObject rule in symbolRules[i])
            {
                FullGrammar = $"{FullGrammar}{rule.name}";
            }
        }
        return FullGrammar;
    }


   public void UpdateVisibleGramar()
    {
        GrammarVisible.text = GramarToString();
    }

    public bool IsAllHeadersWithTwoSymbols()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            if (symbolRules[i].Count > 2)
            {
                return false;
            }
        }
        return true;
    }


    public GameObject SetRuleActive(int index)
    {
        rules[index].SetActive(true);
        return rules[index];
    }

    void SetRuleInactive()
    {
        foreach (GameObject rule in rules)
        {
            rule.SetActive(false);
        }

    }


    public int IsSomeHeaderSelected()
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

    public void SetHeaderNotSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            headerRules[i].GetComponent<TargetMovimentation>().selected = false;
        }
    }

    public void SetHeaderActive()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            headerRules[i].SetActive(true);
        }
    }


    public bool selectHeaderHasMoreThanTwoVariables(int index)
    {
        if (symbolRules[index].Count < 3)
        {
            return false;
        }
        int qtdVariables = 0;
        foreach (GameObject rule in symbolRules[index])
        {
            if (Char.IsUpper(rule.name[0]))
            {
                qtdVariables++;
                if (qtdVariables > 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetCurrentRuleInactive(int currentIndex)
    {
        if (rules[currentIndex])
        {
            rules[currentIndex].SetActive(false);
        }
    }

    public List<GameObject> IsSomeRuleSelected(int index)
    {
        List<GameObject> selectedRules = new List<GameObject>();
        foreach (GameObject rule in symbolRules[index])
        {
            if (rule.GetComponent<TargetMovimentation>().selected)
            {
                selectedRules.Add(rule);
            }
        }
        return selectedRules;
    }


    public void SetTargetRulesActive(int index)
    {
        foreach (GameObject rule in symbolRules[index])
        {
            rule.SetActive(true);
        }
    }

    void SetAllTargetRulesActive()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                rule.SetActive(true);
            }
        }
    }

    void SetAllTargetRulesNotSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                rule.GetComponent<TargetMovimentation>().selected = false;
            }
        }
    }

    bool FindTwoRulesIntoGrammar(string rule1, string rule2)
    {
        GameObject lastRule = null;
        GameObject currentRule = null;
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                lastRule = currentRule;
                currentRule = rule;
                if (lastRule != null && currentRule != null)
                {
                    if (lastRule.name == rule1 && currentRule.name == rule2)
                    {
                        return true;
                    }
                }
            }
            lastRule = null;
            currentRule = null;
        }
        return false;
    }

    public int CountTwoRulesIntoGrammar(string rule1, string rule2)
    {
        int qtd = 0;
        GameObject lastRule = null;
        GameObject currentRule = null;
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                lastRule = currentRule;
                currentRule = rule;
                if (lastRule != null && currentRule != null)
                {
                    if (lastRule.name == rule1 && currentRule.name == rule2)
                    {
                        qtd++;
                    }
                }
            }
            lastRule = null;
            currentRule = null;
        }
        return qtd;
    }

    public void SetHeaderRuleActive()
    {
        parentHeaderRules.SetActive(true);
    }

    public void SetHeaderRuleInactive()
    {
        parentHeaderRules.SetActive(false);
    }
}