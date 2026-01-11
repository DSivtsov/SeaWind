import { notifications } from '@mantine/notifications';

export function showToast(message: string) {
    notifications.show({ message });
}

export function showSuccess(message: string) {
    notifications.show({
        message,
        color: 'green',
    });
}

export function showSuccessWithTitle(title: string, message: string) {
    notifications.show({
        title,
        message,
        color: 'green',
    });
}

export function showError(message: string) {
    notifications.show({
        message,
        color: 'red',
    });
}
