# %% [markdown]
# # XG Boost Modelling
# 

# 

import pandas as pd
import numpy as np

xg_df = pd.read_csv("Set B Corporate Rating - marketvalue3classes.csv") # to read the data from the csv
xg_df # to call

# %%
# Step 2: Column Filter Node - remove unnecessary columns (axis=1 removes columns)
xg_df= xg_df.drop(
    # Include columns to remove here
    ['Total Revenue'],  # Specify 'Total Revenue' for exclusion
    axis=1, 
    errors='ignore'  # Prevent errors if the column is not present
)

xg_df  # Display the filtered dataframe

# %%
#linear correlation
import seaborn as sns
import matplotlib.pyplot as plt

numeric_data = xg_df.select_dtypes(include=[np.number])  # Select only numeric columns

# Compute the correlation matrix
correlation_matrix = numeric_data.corr()

# Create a heatmap using Seaborn
plt.figure(figsize=(12, 10))  # Set the figure size
sns.heatmap(
    correlation_matrix,
    annot=False,  # Do not display correlation values in the cells
    cmap='coolwarm',  
    vmin=-1, vmax=1,  # Set correlation range
    cbar_kws={'label': 'Correlation'},  # Add a color bar
    square=True  # Make cells square
)

# Customize the plot
plt.title('Correlation Heatmap', fontsize=16)
plt.xticks(rotation=90)  # Rotate x-axis labels for better readability
plt.yticks(rotation=0)   # Keep y-axis labels horizontal
plt.tight_layout()  # Adjust spacing

# Show the heatmap
plt.show()

# %%
# Define the feature matrix by dropping the target column
X_filtered = xg_df.drop(columns=['Rating level'])

# Define the target variable
y = xg_df['Rating level']

# %%
pip install xgboost

# %%
# parameter optimization loop
# Define the range for sigma
sigma_values = np.arange(0.3, 0.6 + 0.1, 0.1)  # Start, Stop (inclusive), Step
print("Sigma values to evaluate:", sigma_values)

# %%
# Store the results
results = []

for sigma in sigma_values:
    print(f"Training with sigma = {sigma}")
    
    # Define the model with sigma as the hyperparameter (example usage in 'subsample')
    model = XGBClassifier(subsample=sigma, random_state=42)
    
    # Perform cross-validation (using StratifiedKFold from your earlier setup)
    fold_accuracies = []
    for train_index, test_index in skf.split(X_filtered, y_encoded):
        X_train, X_test = X_filtered.iloc[train_index], X_filtered.iloc[test_index]
        y_train, y_test = y_encoded[train_index], y_encoded[test_index]
        
        # Train the model
        model.fit(X_train, y_train)
        
        # Make predictions
        y_pred = model.predict(X_test)
        
        # Calculate accuracy
        accuracy = accuracy_score(y_test, y_pred)
        fold_accuracies.append(accuracy)
    
    # Store the average accuracy for this sigma
    avg_accuracy = np.mean(fold_accuracies)
    results.append((sigma, avg_accuracy))
    print(f"Sigma = {sigma}, Average Accuracy = {avg_accuracy:.4f}")

# Display the best sigma value
best_sigma, best_accuracy = max(results, key=lambda x: x[1])
print(f"\nBest Sigma: {best_sigma}, Best Average Accuracy: {best_accuracy:.4f}")

# %%
# x-partitioner and xgboost tree learner
from sklearn.model_selection import StratifiedKFold
from sklearn.metrics import accuracy_score
from xgboost import XGBClassifier
from sklearn.preprocessing import LabelEncoder
import numpy as np

# Step 1: Encode the target labels
label_encoder = LabelEncoder()
y_encoded = label_encoder.fit_transform(y)  # Encode 'High Risk', 'Low Risk', etc. to 0, 1, 2

# Number of validation folds
num_folds = 38  # Adjust based on your needs
skf = StratifiedKFold(n_splits=num_folds, shuffle=True, random_state=42)

# Store results for both training and testing accuracies
train_accuracies = []
test_accuracies = []


# Initialize lists for X-Aggregator
all_y_test = []
all_y_test_pred = []


# Cross-validation loop
for fold, (train_index, test_index) in enumerate(skf.split(X_filtered, y_encoded), start=1):
    # Split the data into training and testing sets
    X_train, X_test = X_filtered.iloc[train_index], X_filtered.iloc[test_index]
    y_train, y_test = y_encoded[train_index], y_encoded[test_index]
    
    # Define the XGBoost model
model = XGBClassifier(
    max_depth=3,             # Maximum depth of trees
    learning_rate=0.3,       # Eta (learning rate)
    min_child_weight=1,      # Minimum child weight
    gamma=0,                 # Minimum loss reduction (Gamma)
    reg_lambda=5,            # L2 regularization term (Lambda)
    reg_alpha=5,             # L1 regularization term (Alpha)
    scale_pos_weight=1,      # Scale positive weight
    max_delta_step=0,        # Maximum delta step
    subsample=1,             # Subsampling rate
    colsample_bytree=1,      # Column sampling rate by tree
    objective='multi:softmax', # Multi-class classification objective
    booster='gbtree',        # Booster type
    tree_method='auto',      # Tree construction method
    grow_policy='depthwise', # Growth policy
    n_estimators=100,        # Number of trees (set this explicitly if needed)
    random_state=42          # For reproducibility
)

# Train the model
model.fit(X_train, y_train) # xgboost tree learner
    
# Predict on training data (First XGBoost Predictor)
y_train_pred = model.predict(X_train)
train_accuracy = accuracy_score(y_train, y_train_pred)
train_accuracies.append(train_accuracy)

# Predict on test data (Second XGBoost Predictor)
y_test_pred = model.predict(X_test)
test_accuracy = accuracy_score(y_test, y_test_pred)
test_accuracies.append(test_accuracy)
    
# Store predictions for X-Aggregator
all_y_test.extend(y_test)
all_y_test_pred.extend(y_test_pred)
    
# Print fold-wise results
print(f"Fold {fold}: Training Accuracy = {train_accuracy:.4f}, Test Accuracy = {test_accuracy:.4f}")


# Calculate and print average accuracies
average_train_accuracy = np.mean(train_accuracies)
average_test_accuracy = np.mean(test_accuracies)
print(f"\nAverage Training Accuracy: {average_train_accuracy:.4f}")
print(f"Average Test Accuracy: {average_test_accuracy:.4f}")

# %%
from sklearn.metrics import classification_report, confusion_matrix


# Aggregate results (X-Aggregator functionality)
print("\nOverall Classification Report:")
print(classification_report(all_y_test, all_y_test_pred, target_names=label_encoder.classes_))

# Optional: Confusion Matrix
print("\nConfusion Matrix:")
print(confusion_matrix(all_y_test, all_y_test_pred))


# %%
from sklearn.metrics import classification_report, confusion_matrix, accuracy_score
import numpy as np

# Confusion Matrix
print("\nAggregated Confusion Matrix:")
conf_matrix = confusion_matrix(all_y_test, all_y_test_pred)
print(conf_matrix)

# Display confusion matrix as percentages
conf_matrix_percentages = conf_matrix.astype('float') / conf_matrix.sum(axis=1)[:, np.newaxis] * 100
print("\nAggregated Confusion Matrix (Percentages):")
print(np.round(conf_matrix_percentages, 2))

# Classification Report
print("\nAggregated Classification Report:")
print(classification_report(all_y_test, all_y_test_pred, target_names=label_encoder.classes_))

# Overall Accuracy
overall_accuracy = accuracy_score(all_y_test, all_y_test_pred)
print(f"\nAggregated Overall Accuracy: {overall_accuracy:.2%}")

# %%
# Confusion Matrix for Test Data
print("\nAggregated Confusion Matrix (Testing):")
test_conf_matrix = confusion_matrix(all_y_test, all_y_test_pred)
print(test_conf_matrix)

# Display confusion matrix as percentages
test_conf_matrix_percentages = test_conf_matrix.astype('float') / test_conf_matrix.sum(axis=1)[:, np.newaxis] * 100
print("\nAggregated Confusion Matrix (Percentages):")
print(np.round(test_conf_matrix_percentages, 2))

# Classification Report for Test Data
print("\nAggregated Classification Report (Testing):")
print(classification_report(all_y_test, all_y_test_pred, target_names=label_encoder.classes_))

# Overall Testing Accuracy
test_accuracy = accuracy_score(all_y_test, all_y_test_pred)
print(f"\nAggregated Overall Testing Accuracy: {test_accuracy:.2%}")

# Cohen's Kappa
kappa = cohen_kappa_score(all_y_test, all_y_test_pred)
print(f"Cohen's Kappa: {kappa:.3f}")

# Correctly and Incorrectly Classified Counts
correctly_classified = sum(np.array(all_y_test) == np.array(all_y_test_pred))
incorrectly_classified = len(all_y_test) - correctly_classified
print(f"Correctly Classified: {correctly_classified}")
print(f"Incorrectly Classified: {incorrectly_classified}")

# %%
# Cross-validation setup
skf = StratifiedKFold(n_splits=38, shuffle=True, random_state=42)

# Store results
results = []

param_grid = {
    'max_depth': [3, 5, 7],          # Depth of the trees
    'learning_rate': [0.1, 0.3],     # Learning rate (eta)
    'n_estimators': [50, 100],       # Number of trees
    'subsample': [0.8, 1.0],         # Subsample ratio (fraction of samples used for training each tree)
    'colsample_bytree': [0.8, 1.0]   # Fraction of features used for building each tree
}

# Loop through parameter combinations
for max_depth in param_grid['max_depth']:
    for learning_rate in param_grid['learning_rate']:
        for n_estimators in param_grid['n_estimators']:
            for subsample in param_grid['subsample']:
                for colsample_bytree in param_grid['colsample_bytree']:
                    test_accuracies = []
                    
                    # Cross-validation loop
                    for train_index, test_index in skf.split(X_filtered, y_encoded):
                        X_train, X_test = X_filtered.iloc[train_index], X_filtered.iloc[test_index]
                        y_train, y_test = y_encoded[train_index], y_encoded[test_index]
                        
                        # Define the model
                        model = XGBClassifier(
                            max_depth=max_depth,
                            learning_rate=learning_rate,
                            n_estimators=n_estimators,
                            subsample=subsample,
                            colsample_bytree=colsample_bytree,
                            random_state=42
                        )
                        model.fit(X_train, y_train)
                        
                        # Test accuracy
                        y_test_pred = model.predict(X_test)
                        test_accuracies.append(accuracy_score(y_test, y_test_pred))
                    
                    # Average test accuracy for this parameter combination
                    avg_test_accuracy = np.mean(test_accuracies)
                    
                    # Store the result
                    results.append({
                        'max_depth': max_depth,
                        'learning_rate': learning_rate,
                        'n_estimators': n_estimators,
                        'subsample': subsample,
                        'colsample_bytree': colsample_bytree,
                        'test_accuracy': avg_test_accuracy
                    })

# Find the best parameter combination based on test accuracy
best_result = max(results, key=lambda x: x['test_accuracy'])

print("\nBest Parameters:")
print(best_result)

# %%



