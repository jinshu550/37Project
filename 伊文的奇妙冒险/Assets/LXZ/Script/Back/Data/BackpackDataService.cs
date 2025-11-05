using System.Collections;
using System.Collections.Generic;

public class BackpackDataService
{
    // 根据分类获取数据
    public List<CardBasicInformation> GetCategoryData(BackpackUIManager.CategoryType category)
    {
        switch (category)
        {
            case BackpackUIManager.CategoryType.BackFunction:
                return InventoryManager.Instance.GetBackAllFunctionCards();

            case BackpackUIManager.CategoryType.BodyType1:
                return InventoryManager.Instance.GetBodyType1Cards();
            case BackpackUIManager.CategoryType.BodyType2:
                return InventoryManager.Instance.GetBodyType2Cards();
            case BackpackUIManager.CategoryType.BodyType3:
                return InventoryManager.Instance.GetBodyType3Cards();
            default:
                return new List<CardBasicInformation>();
        }
    }
}