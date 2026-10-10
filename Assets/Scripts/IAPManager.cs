using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour
{
    public static IAPManager instance;
    private StoreController storeController;
    private IExtensionProvider storeExtensionProvider;
    public UnityEvent OnPurchaseSuccess;
    public UnityEvent OnPurchaseFailure;

    public List<string> productIDs = new List<string>(); // To store product IDs like "coins300", "coins1000"
    private bool isPurchaseInProgress = false;

    public bool IsInitializeDone = false;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        InitializePurchasing();
        DontDestroyOnLoad(gameObject);
    }

    public async void InitializePurchasing()
    {
        if (IsInitialized()) return;

        try
        {
            var options = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options); // Initialize Unity Services

            // Connect store controller after initialization
            storeController = UnityIAPServices.StoreController();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            await storeController.Connect();

            // Build product definitions
            var initialProductToFetch = BuildProductDefinitions();

            storeController.FetchProducts(initialProductToFetch);
        }
        catch (Exception e)
        {
            Debug.LogError($"Initialization failed with exception: {e}");
        }
    }

    private void OnPurchaseFailed(FailedOrder order)
    {
    }

    private void OnPurchaseConfirmed(Order obj)
    {
    }

    private void OnPurchasePending(PendingOrder obj)
    {
        
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitializeDone = true;
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription description)
    {
    }


    private void OnProductsFetchFailed(ProductFetchFailed failed)
    {
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
    }

    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    // Handle successful product fetch
    private void OnProductsFetched(List<Product> products)
    {
        Debug.Log("Products fetched successfully.");
        foreach (var product in products)
        {
            Debug.Log($"Product available: {product.definition.id}, Price: {product.metadata.localizedPriceString}");
        }
    }

    private void OnPurchaseDeferred(DeferredOrder order)
    {

    }


    // Purchase product method
    public void PurchaseProduct(string productId)
    {
        if (isPurchaseInProgress)
        {
            Debug.LogWarning("Purchase is already in progress.");
            return;
        }

        if (storeController != null)
        {
            Product product = storeController.GetProductById(productId);
            if (product != null && product.availableToPurchase)
            {
                isPurchaseInProgress = true;
                storeController.PurchaseProduct(product);
            }
            else
            {
                Debug.LogError($"Product {productId} is not available for purchase.");
            }
        }
        else
        {
            Debug.LogError("Store controller is not initialized.");
        }
    }

    
    // Helper method to build product definitions (replace with your own logic)
    private List<ProductDefinition> BuildProductDefinitions()
    {
        List<ProductDefinition> products = new List<ProductDefinition>();

        foreach (var productId in productIDs)
        {
            ProductDefinition product = new ProductDefinition(productId, ProductType.Consumable);
            products.Add(product);
        }

        return products;
    }

  
}
