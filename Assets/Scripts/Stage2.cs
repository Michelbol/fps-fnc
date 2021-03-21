using UnityEngine;
using UnityEngine.UI;

public class Stage2 : MonoBehaviour
{

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

    public GameObject AmmoCounterPannel;

    public GameObject Tutorial;

    public GameObject Target;

    public Grammar grammar;

    // Start is called before the first frame update
    void Start()
    {
        TutorialObjectTitle.GetComponent<Text>().text = TutorialTitle;
        TutorialObjectRule.GetComponent<Text>().text = TutorialRule;
        TutorialObjectHow.GetComponent<Text>().text = TutorialHow;
        TutorialObjectReload.GetComponent<Text>().text = TutorialReload;

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
        AmmunationController.GameEnable = true;
        Target.SetActive(true);
    }

    public void FinishTutorial()
    {
        TutorialOk = true;
        Cursor.visible = false;
        Tutorial.SetActive(false);
    }
}

