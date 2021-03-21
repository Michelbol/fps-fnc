using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage2 : MonoBehaviour
{
    [SerializeField] Text GenericText;

    public bool TutorialOk = false;

    private string TutorialTitle = "Fase 2";

    public GameObject TutorialObjectTitle;

    private string TutorialRule = "- Os simbolos terminais precisam de uma regra própria, contendo apenas ele.";

    public GameObject TutorialObjectRule;

    private string TutorialHow = "- Para substituir, acerte os alvos!";

    public GameObject TutorialObjectHow;

    private string TutorialReload = "- Deu ruim? Recarregue para reiniciar (R)";

    public GameObject TutorialObjectReload;

    private AmmunationController AmmunationController;

    // Symbol not terminal(header rule) selected 
    private GameObject currentRule;
    // Index of symbol not terminal(header rule) selected
    private int currentIndex = -1;

    public GameObject AmmoCounterPannel;
    
    private GameObject currentReplaceVariable;

    private AlertController alertController;

    public GameObject alert;

    public GameObject Tutorial;

    public GameObject Target;
    
    public GameObject TimerLabel;

    public Grammar grammar;

    private int Step = 0;

    private int selectedHeader;

    private Utils utils = new Utils();

    private bool setupStep = false;

    // Start is called before the first frame update
    void Start()
    {
        TutorialObjectTitle.GetComponent<Text>().text = TutorialTitle;
        TutorialObjectRule.GetComponent<Text>().text = TutorialRule;
        TutorialObjectHow.GetComponent<Text>().text = TutorialHow;
        TutorialObjectReload.GetComponent<Text>().text = TutorialReload;

        alertController = alert.GetComponent<AlertController>();
        AmmunationController = AmmoCounterPannel.GetComponent<AmmunationController>();
        AmmunationController.GameEnable = false;
        grammar.ParseGramar();
        grammar.UpdateVisibleGramar();
    }

    void resetGame()
    {
        grammar.ParseGramar();
        Target.SetActive(false);
        AmmunationController.GameEnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TutorialOk)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetGame();
            return;
        }
        if (Step == 0)
        {
            GameObject[] activeObjects = new GameObject[1];
            activeObjects[0] = Target;

            GameObject[] inactiveObjects = new GameObject[0];

            if (utils.startTimer(activeObjects, inactiveObjects, TimerLabel, 3))
            {
                return;
            }
            Target.SetActive(true);
            AmmunationController.GameEnable = true;
            changeStep(1);
        }
        if (Step == 1)
        {
            if (!setupStep)
            {
                grammar.SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                if (!grammar.selectHeaderHasOneTerminal(indexHeader))
                {
                    //AVISO: Gramática não pode ser substituida
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                    alertController.startTimer("Essa regra não tem um terminal!");
                    return;
                }
                selectedHeader = indexHeader;
                grammar.SetHeaderRuleInactive();
                grammar.SetHeaderNotSelected();
                grammar.SetHeaderActive();
                changeStep(2);
            }
        }
        if (Step == 2)
        {
            if (!setupStep)
            {
                currentRule = grammar.SetRuleActive(selectedHeader);
                currentIndex = selectedHeader;
                setupStep = true;
            }
            List<GameObject> selectedObjects = grammar.IsSomeRuleSelected(selectedHeader);
            if (selectedObjects.Count > 0)
            {
                if (grammar.findRuleWithOnlyTerminal(selectedObjects[0].name))
                {
                    //Aviso: Esses simbolos não podem ser substituidos
                    alertController.startTimer("Esses simbolos já tem uma variável terminal");
                    grammar.SetTargetRulesActive(selectedHeader);
                    grammar.SetRuleNotSelected(selectedHeader);
                    grammar.SetTargetRulesActiveAndNotSelectedAndFixPosition(selectedHeader);
                    GenericText.text = "";
                    grammar.SetCurrentRuleInactive(currentIndex);
                    changeStep(1);
                    return;
                }
                currentReplaceVariable = selectedObjects[0];
                changeStep(3);
                grammar.SetCurrentRuleInactive(currentIndex);
            }
        }
        if (Step == 3)
        {
            if (!setupStep)
            {
                GameObject newRule = grammar.replaceTerminalRuleByDefaultRule(currentReplaceVariable, currentIndex);
                grammar.createNewHeaderWithOneRule(newRule.name, currentReplaceVariable.name);
                grammar.UpdateVisibleGramar();
                grammar.SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
            }
        }
    }

    void changeStep(int step)
    {
        grammar.UpdateVisibleGramar();
        grammar.SetHeaderNotSelected();
        Step = step;
        setupStep = false;
    }

    public void FinishTutorial()
    {
        TutorialOk = true;
        Cursor.visible = false;
        Tutorial.SetActive(false);
    }
}

