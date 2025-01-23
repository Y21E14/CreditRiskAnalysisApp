# TESTING OF ALL FEATURES

import numpy as np
import pandas as pd
from joblib import load

# Backend calculation functions (calculations performed on user input data)
# Calculates the Debt to Equity Ratio
def calculate_debt_to_equity_ratio(total_long_term_debt, total_debt_current_liabilities, total_stockholders_equity):
    return (total_long_term_debt - total_debt_current_liabilities) / total_stockholders_equity

# Calculates Net Cash Flow from operating and financing activities
def calculate_net_cash_flow(operating_activities, financing_activities):
    return operating_activities + financing_activities

# Calculates Gross Profit Margin as a percentage
def calculate_gross_profit_margin(gross_profit_loss, total_revenue):
    return (gross_profit_loss / total_revenue) * 100

# Calculates Earnings Before Interest (EBTI) to Total Asset ratio
def calculate_ebti_total_asset(earnings_before_interest, total_asset):
    return earnings_before_interest / total_asset

# Calculates Earnings Before Interest (EBTI) to Revenue ratio
def calculate_ebti_revenue(earnings_before_interest, total_revenue):
    return earnings_before_interest / total_revenue

# Calculates Total Stockholders' Equity
def calculate_total_stockholders_equity(total_asset, total_liabilities):
    return total_asset - total_liabilities

# Calculates the Working Capital Ratio
def calculate_working_capital_ratio(total_asset, cash, total_inventories, total_long_term_debt, total_debt_current_liabilities):
    return (total_asset - (cash + total_inventories)) / (total_long_term_debt - total_debt_current_liabilities)

# Calculates Debt Service Coverage Ratio
def calculate_debt_service_coverage_ratio(earnings_before_interest, total_long_term_debt, total_debt_current_liabilities):
    return earnings_before_interest / (total_long_term_debt + total_debt_current_liabilities)

# Generate test data to simulate user inputs
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

# Perform backend calculations based on the test data

# Calculate Net Cash Flow
test_data["Net Cash Flow"] = calculate_net_cash_flow(
    test_data["Operating Activities - Net Cash Flow"].iloc[0], 
    test_data["Financing Activities - Net Cash Flow"].iloc[0]
)

# Calculate Gross Profit Margin
test_data["Gross Profit Margin"] = calculate_gross_profit_margin(
    test_data["Gross Profit (Loss)"].iloc[0],
    test_data["Total Revenue"].iloc[0]
)

# Calculate Earnings Before Tax and Interest to Total Asset ratio
test_data["EBTI/Total Asset"] = calculate_ebti_total_asset(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Asset"].iloc[0]
)

# Calculate Earnings Before Tax and Interest to Revenue ratio
test_data["EBTI/Revenue"] = calculate_ebti_revenue(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Revenue"].iloc[0]
)

# Recalculate Total Stockholders' Equity
test_data["Total Stockholders Equity"] = calculate_total_stockholders_equity(
    test_data["Total Asset"].iloc[0],
    test_data["Total Liabilities"].iloc[0]
)

# Calculate Debt to Equity Ratio
test_data["Debt to Equity Ratio"] = calculate_debt_to_equity_ratio(
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0],
    test_data["Total Stockholders Equity"].iloc[0]
)

# Calculate Working Capital Ratio
test_data["Working Capital Ratio"] = calculate_working_capital_ratio(
    test_data["Total Asset"].iloc[0],
    test_data["Cash"].iloc[0],
    test_data["Total Inventories"].iloc[0],
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0]
)

# Calculate Debt Service Coverage Ratio
test_data["Debt Service Coverage Ratio"] = calculate_debt_service_coverage_ratio(
    test_data["Earnings Before Interest (optional)"].iloc[0],
    test_data["Total Long - Term Debt"].iloc[0],
    test_data["Total Debt in Current Liabilities"].iloc[0]
)

# Print the test data after calculations
print("\nTest Data with Calculated Fields:")
print(test_data)

# Load the trained model from the .joblib file
# Use the absolute path for loading the model
model = load("joblib/credit_risk_model.joblib")

# Load the trained LabelEncoder
# ADDED: Load the saved LabelEncoder to map numerical predictions to labels
label_encoder = load("joblib/label_encoder.joblib")

# Rename columns to match model training
# Rename columns to align with the model's expected feature names
test_data.rename(columns={
    "Total Long - Term Debt": "Total Long-Term Debt",
    "Earnings Before Interest (optional)": "Earnings Before Interest",
    "Debt to Equity Ratio": "Debt-to-Equity Ratio"
}, inplace=True)

# Add missing features
# Include 'Market Value - Total - Fiscal' in the missing features list
for feature in ["Total debt/total asset", "Total asset/Total libiilities", "EBTI/total asset",
                "Gross Profit/Revenue", "EBTI/REV", "Sales/Turnover (Net)", "Total Current Asset",
                "Market Value - Total - Fiscal"]:  # Ensure all required features are included
    if feature not in test_data.columns:
        test_data[feature] = 0  # Placeholder values

# Ensure only training features are used
# Filter test_data to include only training features
model_input = test_data[
    ["Total Asset", "Cash", "Total Debt in Current Liabilities", "Total Long-Term Debt",
     "Earnings Before Interest", "Gross Profit (Loss)", "Total Liabilities", "Retained Earnings",
     "Total debt/total asset", "Total asset/Total libiilities", "EBTI/total asset",
     "Gross Profit/Revenue", "EBTI/REV", "Sales/Turnover (Net)", "Total Stockholders Equity",
     "Total Interest and Related Expense", "Market Value - Total - Fiscal", "Total Inventories",
     "Operating Activities - Net Cash Flow", "Financing Activities - Net Cash Flow",
     "Net Cash Flow", "Debt-to-Equity Ratio", "Total Current Asset", "Working Capital Ratio",
     "Debt Service Coverage Ratio", "Gross Profit Margin"]
]

# Predict the risk rating using the trained model
predicted_class = model.predict(model_input)

# Map the numerical prediction to the corresponding label
# Use LabelEncoder to decode the numerical prediction
predicted_label = label_encoder.inverse_transform(predicted_class)

# Output the predicted risk rating
print(f"\nPredicted Risk Rating (Numerical): {predicted_class[0]}")
print(f"Predicted Risk Rating (Label): {predicted_label[0]}")
