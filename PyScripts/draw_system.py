import yaml
import plotly.graph_objs as go
import numpy as np
import os

def read_yaml(file_path):
    with open(file_path, 'r') as file:
        return yaml.safe_load(file)

def create_disk(center, radius, color='green', resolution=50):
    theta = np.linspace(0, 2*np.pi, resolution)
    x = center['x'] + radius * np.cos(theta)
    y = center['y'] + radius * np.sin(theta)
    z = np.full_like(x, center['z'])  
    i = np.zeros(resolution-1, dtype=int)
    j = np.arange(1, resolution, dtype=int)
    k = np.arange(2, resolution+1, dtype=int) % resolution
    return go.Mesh3d(
        x=x,
        y=y,
        z=z,
        i=i,
        j=j,
        k=k,
        color=color,
        opacity=0.8,
        flatshading=True
    )

def create_3d_plot(disks_data, shapetype, size):
    data = []

    for disk in disks_data:
        r = disk['r']
        center = disk['center']
        disk_mesh = create_disk(center, r, color='green')
        data.append(disk_mesh)

    if shapetype == 'Sphere':
        u = np.linspace(0, 2 * np.pi, 100)
        v = np.linspace(0, np.pi, 100)
        x = size * np.outer(np.cos(u), np.sin(v))
        y = size * np.outer(np.sin(u), np.sin(v))
        z = size * np.outer(np.ones(np.size(u)), np.cos(v))
        data.append(
            go.Surface(
                x=x, y=y, z=z,
                colorscale='Blues',
                opacity=0.5,
                name='Sphere'
            )
        )
    elif shapetype == 'Cube':
        vertices = np.array([
            [-size, -size, -size],
            [size, -size, -size],
            [size, size, -size],
            [-size, size, -size],
            [-size, -size, size],
            [size, -size, size],
            [size, size, size],
            [-size, size, size]
        ])
        edges = [
            [0, 1], [1, 2], [2, 3], [3, 0],  
            [4, 5], [5, 6], [6, 7], [7, 4],  
            [0, 4], [1, 5], [2, 6], [3, 7]   
        ]
        for edge in edges:
            x = [vertices[edge[0]][0], vertices[edge[1]][0]]
            y = [vertices[edge[0]][1], vertices[edge[1]][1]]
            z = [vertices[edge[0]][2], vertices[edge[1]][2]]
            data.append(
                go.Scatter3d(
                    x=x, y=y, z=z,
                    mode='lines',
                    line=dict(color='blue', width=2),
                    name='Cube'
                )
            )
    else:
        raise ValueError("Неизвестный тип фигуры. Допустимые значения: 'sphere', 'cube'.")

    fig = go.Figure(data=data)

    fig.update_layout(
        scene=dict(
            xaxis_title='X',
            yaxis_title='Y',
            zaxis_title='Z'
        ),
        title='Визуализация системы',
        title_x=0.5,
        height=800,  
        autosize=True 
    )

    return fig

def save_plot_to_html(output_path):
    disks_data = read_yaml(r'.\DataFiles\particles.yaml')
    params_data = read_yaml(r'.\DataFiles\parameters.yaml')

    shapetype = params_data['shapeType']
    size = params_data['size']
    fig = create_3d_plot(disks_data, shapetype, size)

    fig.write_html(output_path)

if __name__ == "__main__":
    output_path = r".\DataFiles\system.html"
    save_plot_to_html(output_path)