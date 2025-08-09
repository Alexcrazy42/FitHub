import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';

const schema = yup.object({
  email: yup.string().required('Email обязателен').email('Неверный email'),
  password: yup.string().min(6, 'Минимум 6 символов').required('Пароль обязателен'),
}).required();

export function LoginForm() {
  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(schema),
  });

  const onSubmit = (data : unknown) => console.log(data);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('email')} placeholder="Email" />
      {errors.email && <p>{errors.email.message}</p>}

      <input type="password" {...register('password')} placeholder="Password" />
      {errors.password && <p>{errors.password.message}</p>}

      <button type="submit">Войти</button>
    </form>
  );
}