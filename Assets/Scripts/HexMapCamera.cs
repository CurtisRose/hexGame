using UnityEngine;

public class HexMapCamera : MonoBehaviour {

	public float stickMinZoom, stickMaxZoom;

	public float swivelMinZoom, swivelMaxZoom;

	public float moveSpeedMinZoom, moveSpeedMaxZoom;

	public float rotationSpeed;

	Transform swivel, stick;

	public HexGrid grid;

	float zoom = 1f;

	float rotationAngle;

	static HexMapCamera instance;

	bool allowUserInput = true;

	Camera hexCamera;

	public bool occlusionCullingEnabled;
	private bool wasOcclusionCullingEnabled;

	// Used to determine how far outside of camera view to render something
	float renderPositionScalarX = 1f;
	float renderPositionScalarY = 1f;

	public static bool Locked {
		set {
			instance.enabled = !value;
		}
	}

	public static void ValidatePosition () {
		instance.AdjustPosition(0f, 0f);
	}

	void Awake () {
		swivel = transform.GetChild(0);
		stick = swivel.GetChild(0);
		hexCamera = GetComponentInChildren<Camera>();
		wasOcclusionCullingEnabled = occlusionCullingEnabled;
	}

	void OnEnable () {
		instance = this;
		ValidatePosition();
	}

	void Update () {
		if (allowUserInput) {
			float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
			if (zoomDelta != 0f) {
				AdjustZoom(zoomDelta);
				PerformOcclusionCulling();
			}

			float rotationDelta = Input.GetAxis("Rotation");
			if (rotationDelta != 0f) {
				AdjustRotation(rotationDelta);
				PerformOcclusionCulling();
			}

			float xDelta = Input.GetAxis("Horizontal");
			float zDelta = Input.GetAxis("Vertical");
			if (xDelta != 0f || zDelta != 0f) {
				AdjustPosition(xDelta, zDelta);
				PerformOcclusionCulling();
			}
		}
	}

	void PerformOcclusionCulling()
    {
		if (occlusionCullingEnabled)
		{
			wasOcclusionCullingEnabled = true;
			foreach (HexGridChunk chunk in grid.GetHexGridChunks())
			{
				Vector3 viewportPosition = hexCamera.WorldToViewportPoint(chunk.GetCenterPosition());
				if (viewportPosition.z < -0)
				{
					chunk.ToggleMeshVisibility(false);
				}
				else if (viewportPosition.x < -0.5 ||
					viewportPosition.x > 1.5 ||
					viewportPosition.y > 1.1 ||
					viewportPosition.y < -2)
				{
					chunk.ToggleMeshVisibility(false);
				} else
                {
					chunk.ToggleMeshVisibility(true);
				}
			}
		} else if (wasOcclusionCullingEnabled)
        {
			wasOcclusionCullingEnabled = false;
			foreach (HexGridChunk chunk in grid.GetHexGridChunks())
			{
				chunk.ToggleMeshVisibility(true);
			}
		}
	}

	void AdjustZoom (float delta) {
		zoom = Mathf.Clamp01(zoom + delta);

		float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

	public void AdjustRotation (float delta) {
		rotationAngle += delta * rotationSpeed * Time.deltaTime;
		if (rotationAngle < 0f) {
			rotationAngle += 360f;
		}
		else if (rotationAngle >= 360f) {
			rotationAngle -= 360f;
		}
		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
	}

	void AdjustPosition (float xDelta, float zDelta) {
		Vector3 direction =
			transform.localRotation *
			new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance =
			Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
			damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition =
			grid.wrapping ? WrapPosition(position) : ClampPosition(position);
	}

	Vector3 ClampPosition (Vector3 position) {
		float xMax = (grid.cellCountX - 0.5f) * HexMetrics.innerDiameter;
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}

	Vector3 WrapPosition (Vector3 position) {
		float width = grid.cellCountX * HexMetrics.innerDiameter;
		while (position.x < 0f) {
			position.x += width;
		}
		while (position.x > width) {
			position.x -= width;
		}

		float zMax = (grid.cellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		grid.CenterMap(position.x);
		return position;
	}

	public void SetZoom(float setZoom) {
		zoom = setZoom;
	}

	public void DisableUserInput() {
		allowUserInput = false;
	}

	public void EnableUserInput() {
		allowUserInput = true;
	}
}