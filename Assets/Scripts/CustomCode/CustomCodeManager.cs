using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class CustomCodeManager : MonoBehaviour
{
    [Header("Managers")]
    public PanelManager panelManager;

    [Header("Compiler")]
    [SerializeField] private Compiler compiler;

    [Header("Game Objects")]
    public TMP_Text compilePopupText;

    public void ExportCode()
    {
        string compileResult = "";
        List<GameObject> blocks = panelManager.blocks;
        bool compiled = compiler.Compile(blocks, ref compileResult);
        if (!compiled)
        {
            compilePopupText.transform.parent.gameObject.SetActive(true);
            compilePopupText.SetText(compileResult);
            return;
        }
        string fileName = "1.bin";
        CellsContainer cellsContainer = new CellsContainer(compiler);
        cellsContainer.Serialize(fileName);
        Debug.Log("Código exportado");
    }

    public void ImportCode()
    {
        string fileName = "1.bin";
        CellsContainer cellsContainer = CellsContainer.Deserialize(fileName);
        // panelManager.LoadCells();
        Debug.Log("Código importado");
        Debug.Log(cellsContainer.memory.Length);
        Debug.Log(cellsContainer.totalCells);
    }

    public void QuitGame()
    {
        panelManager.KillEvents();
        SceneManager.LoadScene("Menu");
    }

    public void ClearBlocks()
    {
        panelManager.Clear();
    }
}
