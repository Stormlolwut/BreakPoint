using UnityEngine;
public interface IDamagable {
    void TakeDamage(int damage);
    float GetHitPoints();
    GameObject GetGameObject();
}
public interface ISellable {
    int SellObject(int money);
    int BuyObject(int money);
}
public interface IUpgradeable {
    int UpgradeObject(int money);
    int TimesUpgraded();
    int UpgradeCost();
    int UpgradeDamage();
}

public interface ISelectable {
    bool IsSelected(bool choice);
}