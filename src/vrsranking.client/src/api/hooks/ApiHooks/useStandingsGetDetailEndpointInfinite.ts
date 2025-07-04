/**
 * Generated by Kubb (https://kubb.dev/).
 * Do not edit manually.
 */

import client from '@kubb/plugin-client/clients/axios'
import type { RequestConfig, ResponseErrorConfig } from '@kubb/plugin-client/clients/axios'
import type { InfiniteData, QueryKey, QueryClient, InfiniteQueryObserverOptions, UseInfiniteQueryResult } from '@tanstack/react-query'
import type { StandingsGetDetailEndpointQueryResponse, StandingsGetDetailEndpointPathParams } from '../../types/StandingsGetDetailEndpoint.ts'
import { infiniteQueryOptions, useInfiniteQuery } from '@tanstack/react-query'

export const standingsGetDetailEndpointInfiniteQueryKey = (
  date: StandingsGetDetailEndpointPathParams['date'],
  filename: StandingsGetDetailEndpointPathParams['filename'],
) => [{ url: '/api/markdown/:date/:filename', params: { date: date, filename: filename } }] as const

export type StandingsGetDetailEndpointInfiniteQueryKey = ReturnType<typeof standingsGetDetailEndpointInfiniteQueryKey>

/**
 * {@link /api/markdown/:date/:filename}
 */
export async function standingsGetDetailEndpointInfinite(
  date: StandingsGetDetailEndpointPathParams['date'],
  filename: StandingsGetDetailEndpointPathParams['filename'],
  config: Partial<RequestConfig> & { client?: typeof client } = {},
) {
  const { client: request = client, ...requestConfig } = config

  const res = await request<StandingsGetDetailEndpointQueryResponse, ResponseErrorConfig<Error>, unknown>({
    method: 'GET',
    url: `/api/markdown/${date}/${filename}`,
    ...requestConfig,
  })
  return res.data
}

export function standingsGetDetailEndpointInfiniteQueryOptions(
  date: StandingsGetDetailEndpointPathParams['date'],
  filename: StandingsGetDetailEndpointPathParams['filename'],
  config: Partial<RequestConfig> & { client?: typeof client } = {},
) {
  const queryKey = standingsGetDetailEndpointInfiniteQueryKey(date, filename)
  return infiniteQueryOptions<StandingsGetDetailEndpointQueryResponse, ResponseErrorConfig<Error>, StandingsGetDetailEndpointQueryResponse, typeof queryKey>({
    enabled: !!(date && filename),
    queryKey,
    queryFn: async ({ signal }) => {
      config.signal = signal
      return standingsGetDetailEndpointInfinite(date, filename, config)
    },
    initialPageParam: 1,
    getNextPageParam: (lastPage) => lastPage['pageNumber'],
    getPreviousPageParam: (firstPage) => firstPage['pageNumber'],
  })
}

/**
 * {@link /api/markdown/:date/:filename}
 */
export function useStandingsGetDetailEndpointInfinite<
  TData = InfiniteData<StandingsGetDetailEndpointQueryResponse>,
  TQueryData = StandingsGetDetailEndpointQueryResponse,
  TQueryKey extends QueryKey = StandingsGetDetailEndpointInfiniteQueryKey,
>(
  date: StandingsGetDetailEndpointPathParams['date'],
  filename: StandingsGetDetailEndpointPathParams['filename'],
  options: {
    query?: Partial<InfiniteQueryObserverOptions<StandingsGetDetailEndpointQueryResponse, ResponseErrorConfig<Error>, TData, TQueryKey>> & {
      client?: QueryClient
    }
    client?: Partial<RequestConfig> & { client?: typeof client }
  } = {},
) {
  const { query: { client: queryClient, ...queryOptions } = {}, client: config = {} } = options ?? {}
  const queryKey = queryOptions?.queryKey ?? standingsGetDetailEndpointInfiniteQueryKey(date, filename)

  const query = useInfiniteQuery(
    {
      ...(standingsGetDetailEndpointInfiniteQueryOptions(date, filename, config) as unknown as InfiniteQueryObserverOptions),
      queryKey,
      ...(queryOptions as unknown as Omit<InfiniteQueryObserverOptions, 'queryKey'>),
    },
    queryClient,
  ) as UseInfiniteQueryResult<TData, ResponseErrorConfig<Error>> & { queryKey: TQueryKey }

  query.queryKey = queryKey as TQueryKey

  return query
}