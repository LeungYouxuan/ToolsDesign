using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using OfficeOpenXml;
using SOScripts;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class ReadTable
{
    static ReadTable()
    {
        CreateGameItemConfigByExcel();
    }
    static void CreateGameItemConfigByExcel()
{
    string excelFilePath = "Assets/ExcelForms/GameDesignForm.xlsx"; // 你的Excel文件路径
    FileInfo existingFile = new FileInfo(excelFilePath);
    if (!existingFile.Exists)
    {
        Debug.LogError($"路径{excelFilePath}不存在文件");
        return;
    }
    GameItemPool gameItemPool = ScriptableObject.CreateInstance<GameItemPool>();
    using (ExcelPackage package = new ExcelPackage(existingFile))
    {
        ExcelWorksheet worksheet = package.Workbook.Worksheets["GameItem"];
        if (worksheet == null)
        {
            Debug.Log("WorkSheet not Found: GameItem");
            return;
        }

        for (int i = worksheet.Dimension.Start.Row + 2; i <= worksheet.Dimension.End.Row; i++)
        {
            GameItem gameItem = new GameItem();
            Type type = typeof(GameItem);
            for (int j = worksheet.Dimension.Start.Column; j <= worksheet.Dimension.End.Column; j++)
            {
                string header = worksheet.GetValue(2, j)?.ToString();
                if (string.IsNullOrEmpty(header))
                {
                    Debug.LogWarning($"列 {j} 的标题为空，跳过该列");
                    continue;
                }

                FieldInfo variable = type.GetField(header);
                if (variable == null)
                {
                    Debug.LogWarning($"类 {type.Name} 中未找到字段 {header}，跳过该列");
                    continue;
                }

                string tableVariable = worksheet.GetValue(i, j)?.ToString();
                if (tableVariable == null)
                {
                    Debug.LogWarning($"行 {i} 列 {j} 的数据为空，跳过该列");
                    continue;
                }

                if (variable.Name == "sprite")
                {
                    // 假设Excel文件中存储的小Sprite名称
                    Sprite sprite = Resources.Load<Sprite>($"ArtAssets/food/{tableVariable}");
                    if (sprite == null)
                    {
                        Debug.LogError($"无法找到名称为{tableVariable}的Sprite");
                        continue;
                    }
                    variable.SetValue(gameItem, sprite);
                }
                else
                {
                    try
                    {
                        object value = Convert.ChangeType(tableVariable, variable.FieldType);
                        variable.SetValue(gameItem, value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"设置值时出错：行 {i} 列 {j}，错误信息：{e.Message}");
                    }
                }
            }
            gameItemPool.gameItemListPool.Add(gameItem);
        }
    }
    // 保存生成的 ScriptableObject
    AssetDatabase.CreateAsset(gameItemPool, "Assets/GameConfig/GameItemPool.asset");
    AssetDatabase.SaveAssets();
}

}
