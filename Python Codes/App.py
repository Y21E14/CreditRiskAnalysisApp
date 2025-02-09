import matplotlib
matplotlib.use('Agg')

import pandas as pd
from joblib import load
from flask import Flask, request, jsonify
from flask_cors import CORS
import matplotlib.pyplot as plt
import io
import base64
import shap

app = Flask(__name__)
CORS(app)

# Load the trained XGBoost model and LabelEncoder
model = load("Python Codes/joblib/credit_risk_model.joblib")
label_encoder = load("Python Codes/joblib/label_encoder.joblib")

# Initialize SHAP Explainer
explainer = shap.TreeExplainer(model)

# Backend calculation functions
def calculate_debt_to_equity_ratio(total_long_term_debt, total_debt_current_liabilities, total_stockholders_equity):
    return (total_long_term_debt - total_debt_current_liabilities) / total_stockholders_equity

def calculate_gross_profit_margin(gross_profit_loss, total_revenue):
    return (gross_profit_loss / total_revenue) * 100

def calculate_working_capital_ratio(total_asset, cash, total_inventories, total_long_term_debt, total_debt_current_liabilities):
    return (total_asset - (cash + total_inventories)) / (total_long_term_debt - total_debt_current_liabilities)

def calculate_debt_service_coverage_ratio(earnings_before_interest, total_long_term_debt, total_debt_current_liabilities):
    return earnings_before_interest / (total_long_term_debt + total_debt_current_liabilities)

def generate_shap_plot(shap_values, input_data, plot_type="summary"):
    """Generate SHAP plots (summary or force) and return base64 string."""
    img = io.BytesIO()
    plt.figure(figsize=(10, 6))

    if plot_type == "summary":
        shap.summary_plot(shap_values, input_data, show=False)
    elif plot_type == "force":
        base_value = explainer.expected_value
        if isinstance(base_value, list):  # Multi-class case
            base_value = base_value[0]
            shap_values = shap_values[..., 0]
        shap.force_plot(base_value, shap_values, input_data.iloc[0], matplotlib=True)
    
    plt.savefig(img, format='png', dpi=300, bbox_inches='tight')
    img.seek(0)
    base64_img = base64.b64encode(img.read()).decode('utf-8')
    plt.close()
    
    return base64_img

@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Parse JSON request payload
        data = request.json
        user_input = pd.DataFrame([data])

        # Perform backend calculation
        user_input["Gross Profit Margin"] = round(calculate_gross_profit_margin(
            user_input["Gross Profit (Loss)"].iloc[0],
            user_input["Total Revenue"].iloc[0]
        ), 2)
        user_input["Debt to Equity Ratio"] = round(calculate_debt_to_equity_ratio(
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0],
            user_input["Total Stockholders Equity"].iloc[0]
        ), 2)
        user_input["Working Capital Ratio"] = round(calculate_working_capital_ratio(
            user_input["Total Asset"].iloc[0],
            user_input["Cash"].iloc[0],
            user_input["Total Inventories"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        ), 2)
        user_input["Debt Service Coverage Ratio"] = round(calculate_debt_service_coverage_ratio(
            user_input["Earnings Before Interest"].iloc[0],
            user_input["Total Long-Term Debt"].iloc[0],
            user_input["Total Debt in Current Liabilities"].iloc[0]
        ), 2)

        # Ensure all required features are included
        feature_names = model.get_booster().feature_names
        for feature in feature_names:
            if feature not in user_input.columns:
                user_input[feature] = 0

        # Align input features with the model
        model_input = user_input[feature_names]

        # Predict the risk using the model directly
        predicted_class = model.predict(model_input)
        predicted_label = label_encoder.inverse_transform([int(predicted_class[0])])

        # Generate SHAP values
        shap_values = explainer.shap_values(model_input)

        # Generate SHAP plots
        shap_summary_plot = generate_shap_plot(shap_values, model_input, plot_type="summary")
        shap_force_plot = generate_shap_plot(shap_values, model_input, plot_type="force")

        # Convert numpy types to native Python types
        return jsonify({
            "credit_risk_numerical": int(predicted_class[0]),
            "credit_risk_label": predicted_label[0],
            "calculated_ratios": {
                "Gross Profit Margin": float(user_input["Gross Profit Margin"].iloc[0]),
                "Debt to Equity Ratio": float(user_input["Debt to Equity Ratio"].iloc[0]),
                "Working Capital Ratio": float(user_input["Working Capital Ratio"].iloc[0]),
                "Debt Service Coverage Ratio": float(user_input["Debt Service Coverage Ratio"].iloc[0])
            },
            "shap_plots": {
                "summary_plot": shap_summary_plot,
                "force_plot": shap_force_plot
            }
        })

    except Exception as e:
        return jsonify({"error": str(e)})

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5000, debug=True)
