using System.Collections.Generic;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
	[ContextMenu("Merger")]
	public void MergeSprites()
	{
		List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
		if (spriteRenderers.Count == 0) return;

		// Определяем границы всех спрайтов
		Bounds bounds = spriteRenderers[0].bounds;
		foreach (var sr in spriteRenderers)
		{
			bounds.Encapsulate(sr.bounds);
		}

		int pixelsPerUnit = Mathf.RoundToInt(spriteRenderers[0].sprite.pixelsPerUnit);
		int width = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
		int height = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);
		Texture2D mergedTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);

		foreach (var sr in spriteRenderers)
		{
			Sprite sprite = sr.sprite;
			Texture2D spriteTexture = sprite.texture;
			if (!spriteTexture.isReadable)
			{
				Debug.LogError($"Texture {spriteTexture.name} is not readable. Enable 'Read/Write' in import settings.");
				return;
			}

			Rect spriteRect = sprite.textureRect;
			Color[] pixels = spriteTexture.GetPixels(
				(int)spriteRect.x,
				(int)spriteRect.y,
				(int)spriteRect.width,
				(int)spriteRect.height
			);

			// Вычисляем позицию спрайта в новой текстуре
			Vector2 spriteWorldPos = sr.transform.position;
			Vector2 minBounds = (Vector2)bounds.min;
			Vector2 offset = (spriteWorldPos - minBounds) * pixelsPerUnit;

			// Учитываем поворот спрайта
			Quaternion rotation = sr.transform.rotation;
			Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

			for (int x = 0; x < spriteRect.width; x++)
			{
				for (int y = 0; y < spriteRect.height; y++)
				{
					// Получаем исходный цвет пикселя
					Color color = pixels[y * (int)spriteRect.width + x];
					if (color.a == 0) continue; // Пропускаем прозрачные пиксели

					// Преобразуем локальные координаты в мировые
					Vector3 localPos = new Vector3(x - sprite.pivot.x, y - sprite.pivot.y, 0) / pixelsPerUnit;
					Vector3 rotatedPos = rotationMatrix.MultiplyPoint3x4(localPos);
					Vector2 finalPos = (Vector2)offset + new Vector2(rotatedPos.x, rotatedPos.y) * pixelsPerUnit;

					int xFinal = Mathf.RoundToInt(finalPos.x);
					int yFinal = Mathf.RoundToInt(finalPos.y);

					// **Добавлена проверка границ**
					if (xFinal >= 0 && yFinal >= 0 && xFinal < width && yFinal < height)
					{
						mergedTexture.SetPixel(xFinal, yFinal, color);
					}
					else
					{
						Debug.LogWarning($"Pixel out of bounds: ({xFinal}, {yFinal})");
					}
				}
			}
		}

		mergedTexture.Apply();

		// Создаем новый спрайт
		Sprite mergedSprite = Sprite.Create(mergedTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
		GameObject mergedObject = new GameObject("MergedSprite");
		SpriteRenderer mergedRenderer = mergedObject.AddComponent<SpriteRenderer>();
		mergedRenderer.sprite = mergedSprite;
		mergedObject.transform.position = bounds.center;
	}
}
