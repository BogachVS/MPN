import yaml
import plotly.graph_objects as go
from pathlib import Path

def plot_ratio_and_save_html(yaml_path, output_html_path):
    with open(yaml_path, 'r') as file:
        data = yaml.safe_load(file)  

    if not isinstance(data, list):
        raise ValueError("YAML должен содержать список объектов!")

    layers = []  
    ratios = []  

    for item in data:  
        try:
            layer_id = item["iD"]
            disc_count = item["pointsInDiscCount"]
            total_count = item["pointsInCount"]
            ratio = disc_count / total_count if total_count != 0 else 0
            layers.append(str(layer_id))  
            ratios.append(ratio)
        except Exception as e:
            print(f"Ошибка в элементе {item}: {e}")

    fig = go.Figure()
    fig.add_trace(
        go.Scatter(
            x=layers,
            y=ratios,
            mode='lines+markers',
            line=dict(color='blue', width=2),
            marker=dict(size=8),
            name="Отношение"
        )
    )

    fig.update_layout(
        title="Оценка вероятносного распределения частиц в системе",
        xaxis_title="ID",
        yaxis_title="M/N",
        hovermode="x unified"
    )

    output_path = Path(output_html_path)
    output_path.parent.mkdir(parents=True, exist_ok=True)
    fig.write_html(str(output_path))
    print(f"График сохранен: {output_path.resolve()}")

if __name__ == "__main__":
    plot_ratio_and_save_html(r'.\DataFiles\analyticsDist.yaml', r'.\DataFiles\graphic.html')
