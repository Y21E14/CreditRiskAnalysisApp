import numpy as np
import pandas as pd
import shap
from xgboost import XGBClassifier
from sklearn.datasets import make_classification
from sklearn.model_selection import train_test_split
import matplotlib.pyplot as plt
import joblib

# --- Step 1: Generate Synthetic (Fake) Data ---
X, y = make_classification(n_samples=500, n_features=29, n_informative=10, n_redundant=5, random_state=42)
columns = [f'Feature_{i}' for i in range(X.shape[1])]  # Naming the features

csv_file = 'C:\FYP\CreditRiskAnalysisApp\Python Codes\Set B Corporate Rating_editing.csv'
 # Replace with your CSV file path
csv_columns = pd.read_csv(csv_file, nrows=0).columns.tolist()  # Get only column names

X = pd.DataFrame(X, columns=csv_columns)
y = pd.Series(y, name='Rating Level')

# --- Step 2: Train the XGBoost Model ---
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)
model = XGBClassifier(random_state=42)
model.fit(X_train, y_train)

# --- Step 3: Save and Reload Model using Joblib ---
joblib.dump(model, r'C:\FYP\CreditRiskAnalysisApp\Python Codes\joblib\xgboost_xai_model.joblib')  # Save the model
loaded_model = joblib.load(r'C:\FYP\CreditRiskAnalysisApp\Python Codes\joblib\xgboost_xai_model.joblib')  # Reload the model

# --- Step 4: Explain with SHAP ---
explainer = shap.Explainer(loaded_model, X_train)
shap_values = explainer(X_test)

# --- Step 5: Visualize SHAP Explanation ---
#shap.summary_plot(shap_values, X_test) #original 
#shap.summary_plot(shap_values, X_test, plot_type="bar")

# --- Step 5: Visualize SHAP Explanation with Feature Names ---
shap.summary_plot(shap_values, X_test, feature_names=X_test.columns, plot_type="bar")