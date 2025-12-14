/**
 * API Error Response format
 */
interface ApiErrorResponse {
  messages: string[];
}

/**
 * Extracts error message from API response
 * Handles both JSON error responses with messages array and plain text responses
 */
export async function getErrorMessage(response: Response): Promise<string> {
  try {
    const contentType = response.headers.get('content-type');

    if (contentType?.includes('application/json')) {
      const errorData: ApiErrorResponse = await response.json();
      return errorData.messages?.join(', ') || 'An error occurred';
    } else {
      const errorText = await response.text();
      return errorText || 'An error occurred';
    }
  } catch {
    return 'An error occurred';
  }
}
