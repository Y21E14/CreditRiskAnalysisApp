# FEATURE IMPORTANCE

import numpy as np
import pandas as pd
from joblib import load

# Backend calculation functions
def calculate_debt_to_equity_ratio(total_long_term_debt, total_debt_current_liabilities, total_stockholders_equity):
    return (total_long_term_debt - total_debt_current_liabilities) / total_stockholders_equity

def calculate_net_cash_flow(operating_activities, financing_activities):
    return operating_activities + financing_activities

def calculate_gross_profit_margin(gross_profit_loss, total_revenue):
    return (gross_profit_loss / total_revenue) * 100

def calculate_ebti_total_asset(earnings_before_interest, total_asset):
    return earnings_before_interest / total_asset

def calculate_ebti_revenue(earnings_before_interest, total_revenue):
    return earnings_before_interest / total_revenue

def calculate_total_stockholders_equity(total_asset, total_liabilities):
    return total_asset - total_liabilities

def calculate_working_capital_ratio(total_asset, cash, total_inventories, total_long_term_debt, total_debt_current_liabilities):
    return (total_asset - (cash + total_inventories)) / (total_long_term_debt - total_debt_current_liabilities)

def calculate_debt_service_coverage_ratio(earnings_before_interest, total_long_term_debt, total_debt_current_liabilities):
    return earnings_before_interest / (total_long_term_debt + total_debt_current_liabilities)

# Generate test data to simulate user inputs for Low, Moderate, and High Risk levels
test_data = pd.DataFrame({
   "Total Asset": [1000000],                    # Extremely high total assets
    "Cash": [500000],                            # High cash reserves
    "Total Debt in Current Liabilities": [10],   # Negligible short-term debt
    "Total Long - Term Debt": [20],              # Negligible long-term debt
    "Earnings Before Interest (optional)": [50000], # Very high earnings before interest
    "Gross Profit (Loss)": [100000],             # Extremely high profitability
    "Total Liabilities": [2900],                 # Low total liabilities
    "Retained Earnings": [0],               # Very high retained earnings
    "Total Stockholders Equity": [997100],       # High equity
    "Total Interest and Related Expense": [200], # Minimal interest expense
    "Total Market Value (optional)": [0],  # Very high market value
    "Total Inventories": [500000],               # High inventory levels
    "Total Revenue": [1000000],                  # Extremely high revenue
    "Operating Activities - Net Cash Flow": [50000], # Strong positive operating cash flow
    "Financing Activities - Net Cash Flow": [10000]  # Positive financing cash flow
})

# Perform backend calculations
test_data["Net Cash Flow"] = calculate_net_cash_flow(
    test_data["Operating Activities - Net Cash Flow"].iloc[0],
    test_data["Financing Activities - Net Cash Flow"].iloc[0]
)
test_data["Gross Profit Margin"] = calculate_gross_profit_margin(
    test_data["Gross Profit (Loss)"].iloc[0],
    test_data["Total Revenue"].iloc[0]
)
test_data["EBTI/Total Asset"] = calculate_ebti_total_asset(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Asset"].iloc[0]
)
test_data["EBTI/Revenue"] = calculate_ebti_revenue(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Revenue"].iloc[0]
)
test_data["Total Stockholders Equity"] = calculate_total_stockholders_equity(
    test_data["Total Asset"].iloc[0],
    test_data["Total Liabilities"].iloc[0]
)
test_data["Debt to Equity Ratio"] = calculate_debt_to_equity_ratio(
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0],
    test_data["Total Stockholders Equity"].iloc[0]
)
test_data["Working Capital Ratio"] = calculate_working_capital_ratio(
    test_data["Total Asset"].iloc[0],
    test_data["Cash"].iloc[0],
    test_data["Total Inventories"].iloc[0],
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0]
)
test_data["Debt Service Coverage Ratio"] = calculate_debt_service_coverage_ratio(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0]
)

print("\nTest Data with Calculated Fields:")
print(test_data)

# Load the trained model and LabelEncoder
model = load("joblib/credit_risk_model.joblib")
label_encoder = load("joblib/label_encoder.joblib")

# Retrieve and sort feature importance
feature_importance = model.feature_importances_
feature_names = model.get_booster().feature_names
importance_df = pd.DataFrame({'Feature': feature_names, 'Importance': feature_importance}).sort_values(
    by='Importance', ascending=False
)

print("\nFeature Importance:")
print(importance_df)

# Select top features for testing
top_features = importance_df['Feature'].head(2).tolist()
print(f"\nTop Features Selected for Testing: {top_features}")

# Rename columns to match model training
test_data.rename(columns={
    "Total Long - Term Debt": "Total Long-Term Debt",
    "Earnings Before Interest (optional)": "Earnings Before Interest",
    "Debt to Equity Ratio": "Debt-to-Equity Ratio"
}, inplace=True)

# Ensure test data aligns with model input
for feature in feature_names:
    if feature not in test_data.columns:
        test_data[feature] = 0

# Align the column order of test_data with feature_names
model_input = test_data[feature_names]

# Predict the risk rating
predicted_class = model.predict(model_input)
predicted_label = label_encoder.inverse_transform(predicted_class)

# Output the predictions
print(f"\nPredicted Risk Rating (Numerical): {predicted_class[0]}")
print(f"Predicted Risk Rating (Label): {predicted_label[0]}")
